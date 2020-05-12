using System;
using System.Collections.Generic;

namespace Aura.Script
{
    public class DefaultValueListParser : Parser
    {
        public DefaultValueListParser(Tokenizer tokenizer) : base(tokenizer) { }

        protected DefaultValueNode ParseDefaultValue()
        {
            var name = Expect(TokenType.Identifier);
            Expect(TokenType.Assign);
            var value = ParseValue();
            ContinueWith(TokenType.Semicolon); // the semicolon is optional based on GlobalSettings.def:114 -_-
            return new DefaultValueNode(name.Pos, name.Value, value);
        }

        public IReadOnlyDictionary<string, DefaultValueNode> ParseDefaultValueList()
        {
            var values = new Dictionary<string, DefaultValueNode>();
            while(true)
            {
                var token = Expect(TokenType.Identifier, TokenType.EndOfSource);
                if (token.Type == TokenType.EndOfSource)
                    return values;

                PushBack(token);
                var value = ParseDefaultValue();
                values[value.Name] = value;
            }
        }
    }
}
