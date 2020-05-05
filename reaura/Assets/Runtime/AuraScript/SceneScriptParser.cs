using System;
using System.Collections.Generic;
using System.Linq;

namespace Aura.Script
{
    public class SceneScriptParser : Parser
    {
        public SceneScriptParser(Tokenizer tokenizer) : base(tokenizer) { }

        private CellPropertyNode ParseCellProperty()
        {
            var name = Expect(TokenType.Identifier);
            Expect(TokenType.Assign);
            var value = ParseValue();
            Expect(TokenType.Semicolon);
            return new CellPropertyNode(CalcPos(name), name.Value, value);
        }

        private CellNode ParseCell()
        {
            var name = Expect(TokenType.Identifier);
            Expect(TokenType.Colon);
            var properties = ParseBlockList(TokenType.Identifier, ParseCellProperty);
            return new CellNode(CalcPos(name), name.Value, properties.ToDictionary(p => p.Name, p => p));
        }

        private CellListNode ParseCellList()
        {
            var name = Expect(TokenType.Identifier);
            var cells = ParseBlockList(TokenType.Identifier, ParseCell);
            return new CellListNode(CalcPos(name), name.Value, cells.ToDictionary(c => c.Name, c => c));
        }

        private GraphicNode ParseGraphic()
        {
            var id = Expect(TokenType.Integer);
            Expect(TokenType.Assign);
            return new GraphicNode(CalcPos(id), int.Parse(id.Value), ParseFunctionCall());
        }

        private GraphicListNode ParseGraphicList()
        {
            var name = Expect(TokenType.Identifier);
            var graphics = ParseBlockList(TokenType.Integer, ParseGraphic);
            return new GraphicListNode(CalcPos(name), name.Value, graphics.ToDictionary(g => g.ID, g => g));
        }

        private EntityListNode ParseEntityList()
        {
            var name = Expect(TokenType.Identifier);
            var bracket = Expect(TokenType.BlockBracketOpen);
            var key = Expect(TokenType.Identifier, TokenType.Integer, TokenType.BlockBracketClose);
            PushBack(key, bracket, name);
            if (key.Type == TokenType.Identifier)
                return ParseCellList();
            else // treat empty entity lists as graphic lists, empty cell lists will be too rare to care about
                return ParseGraphicList();
        }

        private EventNode ParseEvent()
        {
            var name = Expect(TokenType.Identifier);
            var action = ParseInstructionBlock();
            return new EventNode(CalcPos(name), name.Value, action);
        }

        public SceneNode ParseSceneScript()
        {
            var entityLists = new List<EntityListNode>();
            var events = new List<EventNode>();
            var firstToken = Peek();
            while (true)
            {
                var token = Expect(TokenType.Identifier, TokenType.EndOfSource);
                if (token.Type == TokenType.EndOfSource)
                    return new SceneNode(
                        CalcPos(firstToken),
                        entityLists.ToDictionary(l => l.Name, l => l),
                        events.ToDictionary(l => l.Name, l => l));

                PushBack(token); // the name
                if (token.Value.StartsWith("&"))
                    entityLists.Add(ParseEntityList());
                else if (token.Value.StartsWith("@"))
                    events.Add(ParseEvent());
                else
                    throw new Exception($"{token.Pos}: Unexpected identifier, expected either an event or an entity list");
            }
        }
    }
}
