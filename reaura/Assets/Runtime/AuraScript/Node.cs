using System;
using System.Collections.Generic;

namespace Aura.Script
{
    public abstract class Node { }

    public class SceneNode : Node
    {
        public IReadOnlyDictionary<string, EntityListNode> EntityLists { get; }
        public IReadOnlyDictionary<string, EventNode> Events { get; }

        public SceneNode(IReadOnlyDictionary<string, EntityListNode> lists, IReadOnlyDictionary<string, EventNode> events)
        {
            EntityLists = lists;
            Events = events;
        }
    }

    public class EventNode : Node
    {
        public string Name { get; }
        public InstructionBlockNode Action { get; }

        public EventNode(string name, InstructionBlockNode action)
        {
            Name = name;
            Action = action;
        }
    }

    public abstract class EntityListNode : Node
    {
        public string Name { get; }

        public EntityListNode(string name)
        {
            Name = name;
        }
    }

    public class GraphicListNode : EntityListNode
    {
        public IReadOnlyDictionary<int, GraphicNode> Graphics { get; }

        public GraphicListNode(string name, IReadOnlyDictionary<int, GraphicNode> graphics) : base(name)
        {
            Graphics = graphics;
        }
    }

    public class CellListNode : EntityListNode
    {
        public IReadOnlyDictionary<string, CellNode> Cells { get; }

        public CellListNode(string name, IReadOnlyDictionary<string, CellNode> cells) : base(name)
        {
            Cells = cells;
        }
    }

    public class GraphicNode : Node
    {
        public int ID { get; }
        public FunctionCallNode Value { get; }

        public GraphicNode(int id, FunctionCallNode value)
        {
            ID = id;
            Value = value;
        }
    }

    public class CellNode : Node
    {
        public string Name { get; }
        public IReadOnlyDictionary<string, CellPropertyNode> Properties { get; }

        public CellNode(string name, IReadOnlyDictionary<string, CellPropertyNode> props)
        {
            Name = name;
            Properties = props;
        }
    }

    public class CellPropertyNode : Node
    {
        public string Name { get; }
        public ValueNode Value { get; }

        public CellPropertyNode(string name, ValueNode value)
        {
            Name = name;
            Value = value;
        }
    }

    public class InstructionBlockNode : Node
    {
        public IEnumerable<InstructionNode> Instructions { get; }

        public InstructionBlockNode(IEnumerable<InstructionNode> instructions)
        {
            Instructions = instructions;
        }
    }

    public abstract class InstructionNode : Node { }

    public class ReturnNode : InstructionNode { }

    public class AssignmentNode : InstructionNode
    {
        public VariableNode Target { get; }
        public ValueNode Value { get; }

        public AssignmentNode(VariableNode target, ValueNode value)
        {
            Target = target;
            Value = value;
        }
    }

    public class FunctionCallNode : InstructionNode
    {
        public string Function { get; }
        public IEnumerable<ValueNode> Arguments { get; }

        public FunctionCallNode(string function, IEnumerable<ValueNode> args)
        {
            Function = function;
            Arguments = args;
        }
    }

    public class IfNode : InstructionNode
    {
        public ConditionNode Condition { get; }
        public InstructionBlockNode Then { get; }
        public InstructionBlockNode Else { get; }

        public IfNode(ConditionNode condition, InstructionBlockNode thenBlock, InstructionBlockNode elseBlock)
        {
            Condition = condition;
            Then = thenBlock;
            Else = elseBlock;
        }
    }

    public enum ComparisonOp
    {
        Equals,
        NotEquals
    }

    public enum LogicalOp
    {
        And,
        Or
    }

    public abstract class ConditionNode : Node { }

    public class ComparisonNode : ConditionNode
    {
        public ValueNode Left { get; }
        public ValueNode Right { get; }
        public ComparisonOp Op { get; }

        public ComparisonNode(ValueNode left, ValueNode right, ComparisonOp op)
        {
            Left = left;
            Right = right;
            Op = op;
        }
    }

    public class LogicalNode : ConditionNode
    {
        public ConditionNode Left { get; }
        public ConditionNode Right { get; }
        public LogicalOp Op { get; }

        public LogicalNode(ConditionNode left, ConditionNode right, LogicalOp op)
        {
            Left = left;
            Right = right;
            Op = op;
        }
    }

    public abstract class ValueNode : Node {}

    public class VariableNode : ValueNode
    {
        public string Set { get; }
        public string Name { get; }

        public VariableNode(string set, string name)
        {
            Name = name;
        }
    }

    public class NumericNode : ValueNode
    {
        public int Value { get; }

        public NumericNode(int value)
        {
            Value = value;
        }
    }

    public class VectorNode : ValueNode
    {
        public int X { get; }
        public int Y { get; }

        public VectorNode(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class StringNode : ValueNode
    {
        public string Value { get; }

        public StringNode(string value)
        {
            Value = value;
        }
    }
}
