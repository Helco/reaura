﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aura.Script
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ScriptFunctionAttribute : Attribute
    {
        public string? AuraName { get; }

        public ScriptFunctionAttribute(string? auraName = null)
        {
            AuraName = auraName;
        }
    }

    public partial class Interpreter
    {
        private struct ArgumentMapping
        {
            public Type csharp;
            public Type aura;
            public Func<ValueNode, object> mapper;

            public override string ToString() => $"C# {csharp.Name} <- Aura {aura.Name}";
        }

        private struct FunctionMapping
        {
            public object? thiz;
            public MethodInfo method;
            public ParameterInfo[] args;
            public bool isAsync;
        }

        private static readonly IReadOnlyDictionary<string, CubeFace> CubeFaceNames = new Dictionary<string, CubeFace>()
        {
            { "BACKK", CubeFace.Back },
            { "FRONT", CubeFace.Front },
            { "LEFTT", CubeFace.Left },
            { "RIGHT", CubeFace.Right },
            { "UPPPP", CubeFace.Up },
            { "DOWNN", CubeFace.Down }
        };

        private List<ArgumentMapping> argumentMappings = new List<ArgumentMapping>()
        {
            new ArgumentMapping
            {
                csharp = typeof(string),
                aura = typeof(StringNode),
                mapper = node => ((StringNode)node).Value
            },
            new ArgumentMapping
            {
                // just a path without prefix
                csharp = typeof(string),
                aura = typeof(VariableNode),
                mapper = node => $"{((VariableNode)node).Set}.{((VariableNode)node).Name}"
            },
            new ArgumentMapping
            {
                csharp = typeof(int),
                aura = typeof(NumericNode),
                mapper = node => (int)((NumericNode)node).Value
            },
            new ArgumentMapping
            {
                csharp = typeof(float),
                aura = typeof(NumericNode),
                mapper = node => (float)((NumericNode)node).Value
            },
            new ArgumentMapping
            {
                csharp = typeof(double),
                aura = typeof(NumericNode),
                mapper = node => ((NumericNode)node).Value
            },
            new ArgumentMapping
            {
                csharp = typeof(bool),
                aura = typeof(NumericNode),
                mapper = node => ((NumericNode)node).Value != 0
            },
            new ArgumentMapping
            {
                csharp = typeof(Vector2),
                aura = typeof(VectorNode),
                mapper = node => new Vector2(((VectorNode)node).X, ((VectorNode)node).Y)
            },
            new ArgumentMapping
            {
                csharp = typeof(CubeFace),
                aura = typeof(StringNode),
                mapper = node =>
                {
                    var stringNode = (StringNode)node;
                    if (!CubeFaceNames.TryGetValue(stringNode.Value, out var face))
                        throw new InvalidDataException($"{node.Position}: Unknown cube face name \"{stringNode.Value}\"");
                    return face;
                }
            }
        };

        private Dictionary<string, FunctionMapping> functionMappings = new Dictionary<string, FunctionMapping>();

        private bool FindArgumentMapping(Type csharp, Type aura, out ArgumentMapping mapping)
        {
            mapping = argumentMappings.SingleOrDefault(m => m.csharp == csharp && m.aura == aura);
            if (mapping.mapper == null)
            {
                mapping = new ArgumentMapping
                {
                    csharp = csharp,
                    aura = aura
                };
                return false;
            }
            return true;
        }

        public void RegisterArgumentMapper(Type csharp, Type aura, Func<ValueNode, object> mapper)
        {
            var mapping = new ArgumentMapping { csharp = csharp, aura = aura, mapper = mapper };
            if (FindArgumentMapping(csharp, aura, out var _))
                throw new InvalidProgramException($"There already exists an argument mapping for {mapping}");
            argumentMappings.Add(mapping);
        }

        private void RegisterFunction(string auraName, object? thiz, MethodInfo method)
        {
            if (functionMappings.ContainsKey(auraName))
                throw new InvalidProgramException($"There already exists a function mapping for {auraName}");
            bool isAsync = method.ReturnType != typeof(void);
            if (isAsync && !typeof(Task).IsAssignableFrom(method.ReturnType))
                throw new ArgumentException("Method has to return either void or Task");

            functionMappings.Add(auraName, new FunctionMapping
            {
                thiz = thiz,
                method = method,
                args = method.GetParameters(),
                isAsync = isAsync
            });
        }

        private async Task Execute(FunctionCallNode call)
        {
            if (!functionMappings.TryGetValue(call.Function, out var map))
                throw new InvalidDataException($"Unknown function {call.Function}");
            if (map.args.Length != call.Arguments.Count())
                throw new InvalidDataException($"Unexpected parameter count, expected {map.args.Length}, got {call.Arguments.Count()}");

            var args = call.Arguments.Select((arg, i) =>
            {
                var auraArg = call.Arguments.ElementAt(i);
                if (auraArg == null)
                    return null;
                if (!FindArgumentMapping(map.args[i].ParameterType, auraArg.GetType(), out var argMap))
                    throw new InvalidDataException($"Unknown argument mapping {argMap}");
                return argMap.mapper(auraArg);
            }).ToArray();

            if (map.isAsync)
                await (Task)map.method.Invoke(map.thiz, args)!;
            else
                map.method.Invoke(map.thiz, args);
        }

        private static readonly Regex FunctionPrefix = new Regex(@"^Scr");
        public void RegisterAllFunctionsIn(object target)
        {
            var type = target.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var method in methods)
            {
                var scriptFunctions = method.GetCustomAttributes<ScriptFunctionAttribute>();
                foreach (var scriptFunction in scriptFunctions)
                    RegisterFunction(scriptFunction.AuraName ?? FunctionPrefix.Replace(method.Name, ""), target, method);
            }
        }

        public void RegisterFunction(string auraName, Action method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0>(string auraName, Action<T0> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1>(string auraName, Action<T0, T1> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2>(string auraName, Action<T0, T1, T2> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3>(string auraName, Action<T0, T1, T2, T3> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4>(string auraName, Action<T0, T1, T2, T3, T4> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4, T5>(string auraName, Action<T0, T1, T2, T3, T4, T5> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4, T5, T6>(string auraName, Action<T0, T1, T2, T3, T4, T5, T6> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4, T5, T6, T7>(string auraName, Action<T0, T1, T2, T3, T4, T5, T6, T7> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction(string auraName, Func<Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0>(string auraName, Func<T0, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1>(string auraName, Func<T0, T1, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2>(string auraName, Func<T0, T1, T2, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3>(string auraName, Func<T0, T1, T2, T3, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4>(string auraName, Func<T0, T1, T2, T3, T4, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4, T5>(string auraName, Func<T0, T1, T2, T3, T4, T5, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4, T5, T6>(string auraName, Func<T0, T1, T2, T3, T4, T5, T6, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
        public void RegisterFunction<T0, T1, T2, T3, T4, T5, T6, T7>(string auraName, Func<T0, T1, T2, T3, T4, T5, T6, T7, Task> method) => RegisterFunction(auraName, method.Target, method.Method);
    }
}
