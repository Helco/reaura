using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aura.Script
{
    public interface IVariableSet
    {
        int this[string name] { get; set; }
    }
    
    public partial class Interpreter
    {

        private Dictionary<string, IVariableSet> variableSets = new Dictionary<string, IVariableSet>();
        private Dictionary<string, Func<int>> globalValues = new Dictionary<string, Func<int>>()
        {
            { "TRUE", () => 1 },
            { "FALSE", () => 0 },
            { "KILLED", () => -1 },
            { "ACTIVE", () => 1 },
            { "NOACTIVE", () => 0 },
        };

        public Interpreter()
        {
            taskFactory = new TaskFactory(taskScheduler);
            RegisterArgumentMapper(typeof(int), typeof(string), v => Evaluate((StringNode)v));
        }

        public Interpreter Clone()
        {
            var clone = new Interpreter();
            clone.variableSets = variableSets.ToDictionary(p => p.Key, p => p.Value);
            clone.argumentMappings = argumentMappings.ToList();
            clone.functionMappings = functionMappings.ToDictionary(p => p.Key, p => p.Value);
            clone.globalValues = globalValues.ToDictionary(p => p.Key, p => p.Value);
            return clone;
        }

        public void RegisterVariableSet(string name, IVariableSet set)
        {
            if (variableSets.ContainsKey(name))
                throw new InvalidProgramException($"Variable set {name} is already registered");
            variableSets[name] = set;
        }

        public void RegisterGlobalValue(string name, Func<int> valueGetter)
        {
            if (globalValues.ContainsKey(name))
                throw new InvalidOperationException($"Global value {name} is already registered");
            globalValues[name] = valueGetter;
        }

        public bool Evaluate(ConditionNode condition)
        {
            if (condition is LogicalNode) return Evaluate((LogicalNode)condition);
            if (condition is ComparisonNode) return Evaluate((ComparisonNode)condition);
            throw new InvalidProgramException("Unknown condition node");
        }

        public bool Evaluate(LogicalNode logical)
        {
            bool left = Evaluate(logical.Left);
            bool right = Evaluate(logical.Right);
            switch(logical.Op)
            {
                case LogicalOp.And: return left && right;
                case LogicalOp.Or: return left || right;
                default: throw new InvalidProgramException("Unknown logical operator");
            }
        }

        public bool Evaluate(ComparisonNode comparison)
        {
            int left = Evaluate(comparison.Left);
            int right = Evaluate(comparison.Right);
            switch(comparison.Op)
            {
                case ComparisonOp.Equals: return left == right;
                case ComparisonOp.NotEquals: return left != right;
                default: throw new InvalidProgramException("Unknown comparison operator");
            }
        }

        public int Evaluate(ValueNode valueNode)
        {
            if (valueNode is NumericNode) return Evaluate((NumericNode)valueNode);
            if (valueNode is VariableNode) return Evaluate((VariableNode)valueNode);
            if (valueNode is StringNode) return Evaluate((StringNode)valueNode);
            if (valueNode is VectorNode) throw new InvalidDataException("Vectors cannot be evaluated");
            throw new InvalidProgramException("Unknown value node");
        }

        public int Evaluate(NumericNode numeric) => (int)numeric.Value;

        public int Evaluate(VariableNode variable)
        {
            if (!variableSets.TryGetValue(variable.Set, out var variableSet))
                throw new InvalidDataException($"Unknown variable set \"{variable.Set}\"");
            return variableSet[variable.Name];
        }

        public int Evaluate(StringNode stringNode)
        {
            if (!globalValues.TryGetValue(stringNode.Value, out var valueGetter))
                throw new InvalidDataException($"Unknown global value {stringNode.Value}");
            return valueGetter();
        }

        private async Task Execute(InstructionBlockNode block, CancellationToken? token = null)
        {
            token ??= CancellationToken.None;
            foreach (var instruction in block.Instructions)
            {
                if (token.Value.IsCancellationRequested)
                    return;
                await Execute(instruction, token);
            }
        }

        private Task Execute(InstructionNode instruction, CancellationToken? token = null) => instruction switch
        {
            _ when instruction is AssignmentNode => Execute((AssignmentNode)instruction),
            _ when instruction is FunctionCallNode => Execute((FunctionCallNode)instruction),
            _ when instruction is ReturnNode => Execute((ReturnNode)instruction),
            _ when instruction is IfNode => Execute((IfNode)instruction, token),
            var _ => throw new NotImplementedException("Unimplemented instruction node")
        };

        private Task Execute(AssignmentNode assignment)
        {
            if (!variableSets.TryGetValue(assignment.Target.Set, out var variableSet))
                throw new InvalidDataException($"Unknown variable set {assignment.Target.Set}");
            variableSet[assignment.Target.Name] = Evaluate(assignment.Value);
            return Task.CompletedTask;
        }

        private Task Execute(ReturnNode _)
        {
            cts?.Cancel();
            return Task.CompletedTask;
        }
        
        private Task Execute(IfNode @if, CancellationToken? token = null)
        { 
            bool condition = Evaluate(@if.Condition);
            if (condition)
                return Execute(@if.Then, token);
            else if (@if.Else != null)
                return Execute(@if.Else, token);
            else
                return Task.CompletedTask;
        }
    }
}
