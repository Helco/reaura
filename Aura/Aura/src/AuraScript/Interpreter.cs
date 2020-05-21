using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aura.Script
{
    public interface IVariableSet
    {
        int this[string name] { get; set; }
    }
    
    public partial class Interpreter
    {
        private static readonly IReadOnlyDictionary<string, int> Constants = new Dictionary<string, int>()
        {
            { "TRUE", 1 },
            { "FALSE", 0 },
            { "ACTIVE", 1 },
            { "NOACTIVE", 0 }
        };

        // Unfortunately this member means that the Interpreter is *not* thread-safe, but it is the most unintrusive way...
        private bool shouldBeExiting = false;
        private Dictionary<string, IVariableSet> variableSets = new Dictionary<string, IVariableSet>();

        public Interpreter()
        {
            RegisterArgumentMapper(typeof(int), typeof(string), v =>
            {
                var stringNode = (StringNode)v;
                if (!Constants.TryGetValue(stringNode.Value, out int value))
                    throw new InvalidDataException($"Unknown constant {stringNode.Value}");
                return value;
            });
        }

        public Interpreter Clone()
        {
            var clone = new Interpreter();
            clone.variableSets = variableSets.ToDictionary(p => p.Key, p => p.Value);
            clone.argumentMappings = argumentMappings.ToList();
            clone.functionMappings = functionMappings.ToDictionary(p => p.Key, p => p.Value);
            return clone;
        }

        public void RegisterVariableSet(string name, IVariableSet set)
        {
            if (variableSets.ContainsKey(name))
                throw new InvalidProgramException($"Variable set {name} is already registered");
            variableSets[name] = set;
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

        public int Evaluate(NumericNode numeric) => numeric.Value;

        public int Evaluate(VariableNode variable)
        {
            if (!variableSets.TryGetValue(variable.Set, out var variableSet))
                throw new InvalidDataException($"Unknown variable set {variable.Set}");
            return variableSet[variable.Name];
        }

        public int Evaluate(StringNode str)
        {
            if (!Constants.TryGetValue(str.Value, out int value))
                throw new InvalidDataException($"Unknown constant {str.Value}");
            return value;
        }

        public void Execute(InstructionBlockNode block)
        {
            shouldBeExiting = false;
            foreach (var instruction in block.Instructions)
            {
                Execute(instruction);
                if (shouldBeExiting)
                    return;
            }
        }

        public void Execute(InstructionNode instruction)
        {
            if (instruction is AssignmentNode) Execute((AssignmentNode)instruction);
            else if (instruction is FunctionCallNode) Execute((FunctionCallNode)instruction);
            else if (instruction is ReturnNode) Execute((ReturnNode)instruction);
            else if (instruction is IfNode) Execute((IfNode)instruction);
            else throw new InvalidDataException("Unknown instruction node");
        }

        public void Execute(AssignmentNode assignment)
        {
            if (!variableSets.TryGetValue(assignment.Target.Set, out var variableSet))
                throw new InvalidDataException($"Unknown variable set {assignment.Target.Set}");
            variableSet[assignment.Target.Name] = Evaluate(assignment.Value);
        }

        public void Execute(ReturnNode _) => shouldBeExiting = true;
        
        public void Execute(IfNode @if)
        { 
            bool condition = Evaluate(@if.Condition);
            if (condition)
                Execute(@if.Then);
            else if (@if.Else != null)
                Execute(@if.Else);
        }
    }
}
