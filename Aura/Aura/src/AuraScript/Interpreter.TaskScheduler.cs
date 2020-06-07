using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aura.Script
{
    public partial class Interpreter
    {
        private class InterpreterScheduler : TaskScheduler
        {
            private Queue<Task> taskQueue = new Queue<Task>(1);
            private bool isContinuing = false;
            public Exception? Exception { get; set; } = null;

            protected override IEnumerable<Task>? GetScheduledTasks() => taskQueue;

            protected override void QueueTask(Task task)
            {
                if (HasRunningTask)
                    throw new InvalidOperationException("Unexpected queue operation, were multiple runs started?");
                taskQueue.Enqueue(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return isContinuing && TryExecuteTask(task);
            }

            public bool HasRunningTask => taskQueue.Count > 0;

            public void Continue()
            {
                isContinuing = true;
                while (taskQueue.TryDequeue(out var task))
                {
                    if (!TryExecuteTask(task))
                        taskQueue.Enqueue(task);
                    else if (task.Exception != null)
                        throw task.Exception;
                    else if (Exception != null)
                        throw Exception;
                }
                isContinuing = false;
            }
        }

        private InterpreterScheduler taskScheduler = new InterpreterScheduler();
        private TaskFactory taskFactory;
        private CancellationTokenSource? cts = null;

        public bool IsReady => !taskScheduler.HasRunningTask;

        public void CancelCurrentExecution()
        {
            if (cts == null)
                return;
            cts.Cancel();
            while (!IsReady)
                taskScheduler.Continue();
            cts = null;
        }

        private void ExecuteSync<TArg>(Func<TArg, CancellationToken?, Task> action, TArg arg)
        {
            // yes, this will be a deadlock if the called functions are actually asynchronous or executed on another task scheduler
            var task = ExecuteAsync(action, arg);
            while (!task.IsCompleted)
                taskScheduler.Continue();
        }

        private Task ExecuteAsync<TArg>(Func<TArg, CancellationToken?, Task> action, TArg arg, CancellationToken? token = null)
        {
            if (cts != null)
                throw new InvalidOperationException("Interpreter is already executing");
            cts = token == null
                ? new CancellationTokenSource()
                : CancellationTokenSource.CreateLinkedTokenSource(token.Value);
            return taskFactory.StartNew(async () =>
            {
                try
                {
                    await action(arg, cts.Token);
                }
                catch(Exception e)
                {
                    taskScheduler.Exception = new AggregateException(e);
                }
                cts = null;
            }, cts.Token);
        }

        public void ExecuteSync(FunctionCallNode callNode) => ExecuteSync(Execute, callNode);
        public void ExecuteSync(InstructionBlockNode blockNode) => ExecuteSync(Execute, blockNode);
        public Task ExecuteAsync(FunctionCallNode callNode, CancellationToken? token = null) => ExecuteAsync(Execute, callNode, token);
        public Task ExecuteAsync(InstructionBlockNode blockNode, CancellationToken? token = null) => ExecuteAsync(Execute, blockNode, token);
    }
}
