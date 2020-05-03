using System;

namespace Aura.Script
{
    public enum TokenType
    {
        Identifier,
        Integer,

        BlockBracketOpen,
        BlockBracketClose,
        ExprBracketOpen,
        ExprBracketClose,
        TupleBracketOpen,
        TupleBracketClose,

        Semicolon,
        Assign,
        Equals,
        LogicalAnd,
        LogicalOr,

        EndOfSource
    }

    public struct Token
    {
        public TokenType Type { get; }
        public ScriptPos Pos { get; }
        public string Value { get; }

        public Token(TokenType type, ScriptPos pos, string value = "")
        {
            Type = type;
            Pos = pos;
            Value = value;
        }
    }
}
