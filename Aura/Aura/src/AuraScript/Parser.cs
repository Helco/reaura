using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aura.Script
{
    public abstract class Parser
    {
        IEnumerator<Token> scanner;
        Stack<Token> backStack = new Stack<Token>();

        protected Parser(Tokenizer tokenizer)
        {
            scanner = tokenizer.GetEnumerator();
        }

        protected void PushBack(params Token[] tokens)
        {
            foreach (var token in tokens)
                backStack.Push(token);
        }

        protected Token Next()
        {
            if (backStack.Any())
                return backStack.Pop();
            scanner.MoveNext();
            return scanner.Current;
        }

        protected Token Peek()
        {
            var token = Next();
            backStack.Push(token);
            return token;
        }

        protected Token? ContinueWith(params TokenType[] types)
        {
            if (types.Contains(Peek().Type))
                return Next();
            return null;
        }

        protected Token Expect(params TokenType[] types)
        {
            var token = ContinueWith(types);
            if (!token.HasValue)
                throw new Exception($"{Peek().Pos}: Unexpected {Peek().Type}, expected one of {string.Join(", ", types)}");
            return token.Value;
        }

        protected ScriptPos CalcPos(Token first) => CalcPos(first.Pos);
        protected ScriptPos CalcPos(ScriptPos first) => Peek().Pos - first;

        protected IEnumerable<T> ParseBlockList<T>(TokenType firstToken, Func<T> parse)
        {
            Expect(TokenType.BlockBracketOpen);
            var list = new List<T>();
            while (true)
            {
                var token = Expect(firstToken, TokenType.BlockBracketClose);
                if (token.Type == TokenType.BlockBracketClose)
                    return list;

                PushBack(token);
                list.Add(parse());
            }
        }

        private static readonly Regex VariableRegex = new Regex(@"^(\w+)\.(\w+)$");
        protected bool TryParseVariable(Token identifier, out VariableNode variable)
        {
            variable = null;
            var match = VariableRegex.Match(identifier.Value);
            if (!match.Success)
                return false;
            variable = new VariableNode(CalcPos(identifier), match.Groups[1].Value, match.Groups[2].Value);
            return true;
        }

        protected VectorNode ParseVector()
        {
            var bracketOpen = Expect(TokenType.TupleBracketOpen);
            var x = Expect(TokenType.Integer);
            Expect(TokenType.Comma);
            var y = Expect(TokenType.Integer);
            Expect(TokenType.TupleBracketClose);
            return new VectorNode(CalcPos(bracketOpen), int.Parse(x.Value), int.Parse(y.Value));
        }

        protected ValueNode ParseValue()
        {
            var token = Expect(TokenType.Identifier, TokenType.Integer, TokenType.TupleBracketOpen);
            if (token.Type == TokenType.Integer)
                return new NumericNode(CalcPos(token), int.Parse(token.Value));
            else if (token.Type == TokenType.TupleBracketOpen)
            {
                PushBack(token);
                return ParseVector();
            }
            else if (TryParseVariable(token, out var variable))
                return variable;
            else
                return new StringNode(CalcPos(token), token.Value);
        }

        protected ComparisonOp ParseComparisonOp()
        {
            var token = Expect(TokenType.Equals, TokenType.NotEquals);
            return token.Type == TokenType.Equals ? ComparisonOp.Equals : ComparisonOp.NotEquals;
        }

        protected ComparisonNode ParseComparison()
        {
            var bracketOpen = Expect(TokenType.ExprBracketOpen);
            var left = ParseValue();
            var op = ParseComparisonOp();
            var right = ParseValue();
            Expect(TokenType.ExprBracketClose);
            return new ComparisonNode(CalcPos(bracketOpen), left, right, op);
        }

        protected LogicalOp ParseLogicalOp()
        {
            var token = Expect(TokenType.LogicalAnd, TokenType.LogicalOr);
            return token.Type == TokenType.LogicalAnd ? LogicalOp.And : LogicalOp.Or;
        }

        protected LogicalNode ParseLogical()
        {
            var bracketOpen = Expect(TokenType.ExprBracketOpen);
            var left = ParseCondition();
            var op = ParseLogicalOp();
            var right = ParseCondition();
            var logical = new LogicalNode(CalcPos(bracketOpen), left, right, op);

            while(true)
            {
                var token = Expect(TokenType.LogicalAnd, TokenType.LogicalOr, TokenType.ExprBracketClose);
                if (token.Type == TokenType.ExprBracketClose)
                    return logical;

                PushBack(token);
                op = ParseLogicalOp();
                right = ParseCondition();
                logical = new LogicalNode(CalcPos(bracketOpen), logical, right, op);
            }
        }

        protected ConditionNode ParseCondition()
        {
            var bracket = Expect(TokenType.ExprBracketOpen);
            var token = Peek();
            PushBack(bracket);

            if (token.Type == TokenType.ExprBracketOpen)
                return ParseLogical();
            else
                return ParseComparison();
        }

        protected FunctionCallNode ParseFunctionCall()
        {
            var function = Expect(TokenType.Identifier);
            Expect(TokenType.ExprBracketOpen);
            var args = new List<ValueNode>();
            while(true)
            {
                var token = Next();
                if (token.Type == TokenType.ExprBracketClose)
                {
                    if (args.Count > 0) // for (constant, constant , )
                        args.Add(null);
                    break;
                }
                else if (token.Type == TokenType.Comma)
                    args.Add(null);
                else
                {
                    PushBack(token);
                    args.Add(ParseValue());
                    
                    token = Expect(TokenType.Comma, TokenType.ExprBracketClose);
                    if (token.Type == TokenType.ExprBracketClose)
                        break;
                }
            }
            Expect(TokenType.Semicolon);
            return new FunctionCallNode(CalcPos(function), function.Value, args);
        }

        protected AssignmentNode ParseAssignment()
        {
            var variableIdentifier = Expect(TokenType.Identifier);
            if (!TryParseVariable(variableIdentifier, out var variable))
                throw new Exception($"{variableIdentifier.Pos}: Expected a variable");
            Expect(TokenType.Assign);
            var value = ParseValue();
            Expect(TokenType.Semicolon);
            return new AssignmentNode(CalcPos(variableIdentifier), variable, value);
        }

        protected IfNode ParseIf()
        {
            var ifKeyword = Expect(TokenType.Identifier);
            var condition = ParseCondition();
            var thenBlock = ParseInstructionBlock();

            var elseToken = ContinueWith(TokenType.Identifier);
            if (!elseToken.HasValue)
                return new IfNode(CalcPos(ifKeyword), condition, thenBlock, null);
            else if (elseToken.Value.Value != "else")
            {
                PushBack(elseToken.Value);
                return new IfNode(CalcPos(ifKeyword), condition, thenBlock, null);
            }

            var elseBlock = ParseInstructionBlock();
            return new IfNode(CalcPos(ifKeyword), condition, thenBlock, elseBlock);
        }

        protected ReturnNode ParseReturn()
        {
            var returnKeyword = Expect(TokenType.Identifier);
            Expect(TokenType.Semicolon);
            return new ReturnNode(CalcPos(returnKeyword));
        }

        protected InstructionNode ParseInstruction()
        {
            var token = Expect(TokenType.Identifier);
            var next = Peek();
            PushBack(token);

            if (token.Value == "if")
                return ParseIf();
            else if (token.Value == "return")
                return ParseReturn();
            else if (next.Type == TokenType.ExprBracketOpen)
                return ParseFunctionCall();
            else if (next.Type == TokenType.Assign)
                return ParseAssignment();
            else
                throw new Exception($"{next.Pos}: Unexpected {next.Type}, expected one of ExprBracketOpen, Assign");
        }

        protected InstructionBlockNode ParseInstructionBlock()
        {
            var bracketOpen = Peek();
            var list = ParseBlockList(TokenType.Identifier, ParseInstruction);
            return new InstructionBlockNode(CalcPos(bracketOpen), list);
        }

        protected PropertyNode ParseProperty()
        {
            var name = Expect(TokenType.Identifier);
            Expect(TokenType.Assign);
            var value = ParseValue();
            Expect(TokenType.Semicolon);
            return new PropertyNode(CalcPos(name), name.Value, value);
        }

        protected ObjectNode ParseObject()
        {
            var name = Expect(TokenType.Identifier);
            ContinueWith(TokenType.Colon); // it is optional in Predmets.prd
            var properties = ParseBlockList(TokenType.Identifier, ParseProperty);
            return new ObjectNode(CalcPos(name), name.Value, properties.ToDictionary(p => p.Name, p => p));
        }

        protected ObjectListNode ParseObjectList()
        {
            var name = Expect(TokenType.Identifier);
            var cells = ParseBlockList(TokenType.Identifier, ParseObject);
            return new ObjectListNode(CalcPos(name), name.Value, cells.ToDictionary(c => c.Name, c => c));
        }
    }
}
