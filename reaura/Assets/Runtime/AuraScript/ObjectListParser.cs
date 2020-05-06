using System.Collections.Generic;

namespace Aura.Script
{
    public class ObjectListParser : Parser
    {
        public ObjectListParser(Tokenizer tokenizer) : base(tokenizer) {}

        public new IEnumerable<ObjectNode> ParseObjectList()
        {
            var objects = new List<ObjectNode>();
            while(true)
            {
                var token = Expect(TokenType.Identifier, TokenType.EndOfSource);
                if (token.Type == TokenType.EndOfSource)
                    return objects;

                PushBack(token);
                objects.Add(ParseObject());
            }
        }
    }
}
