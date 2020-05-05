using System;
using System.Collections.Generic;

namespace Aura.Script
{
    public abstract class Node
    {
        public ScriptPos Position { get; }

        public Node(ScriptPos pos) { Position = pos; }
    }

    public class SceneNode : Node
    {
        public IReadOnlyDictionary<string, EntityListNode> EntityLists { get; }
        public IReadOnlyDictionary<string, EventNode> Events { get; }

        public SceneNode(ScriptPos pos, IReadOnlyDictionary<string, EntityListNode> lists, IReadOnlyDictionary<string, EventNode> events) : base(pos)
        {
            EntityLists = lists;
            Events = events;
        }
    }

    public class EventNode : Node
    {
        public string Name { get; }
        public InstructionBlockNode Action { get; }

        public EventNode(ScriptPos pos, string name, InstructionBlockNode action) : base(pos)
        {
            Name = name;
            Action = action;
        }
    }

    public abstract class EntityListNode : Node
    {
        public string Name { get; }

        public EntityListNode(ScriptPos pos, string name) : base(pos)
        {
            Name = name;
        }
    }

    public class GraphicListNode : EntityListNode
    {
        public IReadOnlyDictionary<int, GraphicNode> Graphics { get; }

        public GraphicListNode(ScriptPos pos, string name, IReadOnlyDictionary<int, GraphicNode> graphics) : base(pos, name)
        {
            Graphics = graphics;
        }
    }

    public class CellListNode : EntityListNode
    {
        public IReadOnlyDictionary<string, CellNode> Cells { get; }

        public CellListNode(ScriptPos pos, string name, IReadOnlyDictionary<string, CellNode> cells) : base(pos, name)
        {
            Cells = cells;
        }
    }

    public class GraphicNode : Node
    {
        public int ID { get; }
        public FunctionCallNode Value { get; }

        public GraphicNode(ScriptPos pos, int id, FunctionCallNode value) : base(pos)
        {
            ID = id;
            Value = value;
        }
    }

    public class CellNode : Node
    {
        public string Name { get; }
        public IReadOnlyDictionary<string, CellPropertyNode> Properties { get; }

        public CellNode(ScriptPos pos, string name, IReadOnlyDictionary<string, CellPropertyNode> props) : base(pos)
        {
            Name = name;
            Properties = props;
        }
    }

    public class CellPropertyNode : Node
    {
        public string Name { get; }
        public ValueNode Value { get; }

        public CellPropertyNode(ScriptPos pos, string name, ValueNode value) : base(pos)
        {
            Name = name;
            Value = value;
        }
    }

    public class InstructionBlockNode : Node
    {
        public IEnumerable<InstructionNode> Instructions { get; }

        public InstructionBlockNode(ScriptPos pos, IEnumerable<InstructionNode> instructions) : base(pos)
        {
            Instructions = instructions;
        }
    }

    public abstract class InstructionNode : Node
    {
        public InstructionNode(ScriptPos pos) : base(pos) { }
    }

    public class ReturnNode : InstructionNode
    {
        public ReturnNode(ScriptPos pos) : base(pos) { }
    }

    public class AssignmentNode : InstructionNode
    {
        public VariableNode Target { get; }
        public ValueNode Value { get; }

        public AssignmentNode(ScriptPos pos, VariableNode target, ValueNode value) : base(pos)
        {
            Target = target;
            Value = value;
        }
    }

    public class FunctionCallNode : InstructionNode
    {
        public string Function { get; }
        public IEnumerable<ValueNode> Arguments { get; }

        public FunctionCallNode(ScriptPos pos, string function, IEnumerable<ValueNode> args) : base(pos)
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

        public IfNode(ScriptPos pos, ConditionNode condition, InstructionBlockNode thenBlock, InstructionBlockNode elseBlock) : base(pos)
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

    public abstract class ConditionNode : Node
    {
        public ConditionNode(ScriptPos pos) : base(pos) { }
    }

    public class ComparisonNode : ConditionNode
    {
        public ValueNode Left { get; }
        public ValueNode Right { get; }
        public ComparisonOp Op { get; }

        public ComparisonNode(ScriptPos pos, ValueNode left, ValueNode right, ComparisonOp op) : base(pos)
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

        public LogicalNode(ScriptPos pos, ConditionNode left, ConditionNode right, LogicalOp op) : base(pos)
        {
            Left = left;
            Right = right;
            Op = op;
        }
    }

    public abstract class ValueNode : Node
    {
        public ValueNode(ScriptPos pos) : base(pos) { }
    }

    public class VariableNode : ValueNode
    {
        public string Set { get; }
        public string Name { get; }

        public VariableNode(ScriptPos pos, string set, string name) : base(pos)
        {
            Name = name;
        }
    }

    public class NumericNode : ValueNode
    {
        public int Value { get; }

        public NumericNode(ScriptPos pos, int value) : base(pos)
        {
            Value = value;
        }
    }

    public class VectorNode : ValueNode
    {
        public int X { get; }
        public int Y { get; }

        public VectorNode(ScriptPos pos, int x, int y) : base(pos)
        {
            X = x;
            Y = y;
        }
    }

    public class StringNode : ValueNode
    {
        public string Value { get; }

        public StringNode(ScriptPos pos, string value) : base(pos)
        {
            Value = value;
        }
    }
}
