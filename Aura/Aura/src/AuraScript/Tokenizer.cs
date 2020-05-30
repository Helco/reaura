using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.Serialization;

namespace Aura.Script
{
    public class Tokenizer : IEnumerable<Token>
    {
        private string file;
        private string text;

        public Tokenizer(string file, string text)
        {
            this.file = file;
            this.text = text;
        }

        public TokenizerEnumerator GetEnumerator() => new TokenizerEnumerator(this);
        IEnumerator<Token> IEnumerable<Token>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public sealed class TokenizerEnumerator : IEnumerator<Token>, IEnumerator
        {
            private static readonly IReadOnlyDictionary<char, TokenType> SingleCharTokens = new Dictionary<char, TokenType>()
            {
                { '{', TokenType.BlockBracketOpen },
                { '}', TokenType.BlockBracketClose },
                { '(', TokenType.ExprBracketOpen },
                { ')', TokenType.ExprBracketClose },
                { '[', TokenType.TupleBracketOpen },
                { ']', TokenType.TupleBracketClose },
                { ';', TokenType.Semicolon },
                { ':', TokenType.Colon },
                { ',', TokenType.Comma }
            };

            private Tokenizer source;
            private TextReader reader = null!; // will be set by Reset in ctor
            private ScriptPos position;
            private bool didSendEoS;

            internal TokenizerEnumerator(Tokenizer source)
            {
                this.source = source;
                Reset();
            }

            public Token Current { get; private set; }
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public void Reset()
            {
                reader = new StringReader(source.text);
                position = new ScriptPos(source.file);
                Current = new Token(TokenType.EndOfSource, position);
                didSendEoS = false;
            }

            private int Peek() => reader.Peek();
            
            private int Read()
            {
                int result = reader.Read();
                position = result == '\n'
                    ? position.NextLine
                    : position.NextColumn;
                return result;
            }

            private void SkipSpaceAndComments()
            {
                int ch = Peek();
                while (ch >= 0)
                {
                    if (char.IsWhiteSpace((char)ch))
                        Read();
                    else if (ch == '*' || ch == '/' || ch == '?')
                        SkipToNextLine();
                    else
                        return;
                    ch = Peek();
                }
            }

            public void SkipToNextLine()
            {
                int ch = Peek();
                while (ch >= 0 && ch != '\n')
                    ch = Read();
            }

            public string ReadWhile(string prefix, Predicate<char> isValid)
            {
                string value = prefix;
                int chI = Peek();
                while (chI >= 0 && isValid((char)chI))
                {
                    value += (char)Read();
                    chI = Peek();
                }
                return value;
            }

            public bool MoveNext()
            {
                SkipSpaceAndComments();
                int chI = Peek();
                if (chI < 0)
                {
                    bool result = !didSendEoS;
                    didSendEoS = true;
                    Current = new Token(TokenType.EndOfSource, position);
                    return result;
                }
                char ch = (char)chI;

                ScriptPos tokenStart = position;
                if (char.IsLetter(ch) || "_.\\$@&%".Contains(ch))
                {
                    string value = "" + (char)Read();
                    if (ch == '&' && Peek() == '&')
                    {
                        Read();
                        Current = new Token(TokenType.LogicalAnd, position - tokenStart);
                        return true;
                    }

                    value = ReadWhile(value, c => char.IsLetterOrDigit(c) || "_.\\".Contains(c));
                    Current = new Token(TokenType.Identifier, position - tokenStart, value);
                    return true;
                }

                if (ch == '\"')
                {
                    Read();
                    string value = ReadWhile("", c => !char.IsControl(c) && c != '\"');
                    if (Peek() != '\"')
                        throw new Exception($"Unexpected symbol '{(char)Peek()}', expected '\"'");
                    Read();
                    Current = new Token(TokenType.Identifier, position - tokenStart, value);
                    return true;
                }

                if (char.IsDigit(ch) || ch == '-')
                {
                    string value = ReadWhile("" + (char)Read(), c => char.IsDigit(c) || c == '.');
                    if (!double.TryParse(value, out var _))
                        throw new Exception("Invalid number format");
                    Current = new Token(TokenType.Number, position - tokenStart, value);
                    return true;
                }

                if (ch == '|')
                {
                    Read();
                    if (Peek() != '|')
                        throw new Exception($"Unexpected symbol '{(char)Peek()}', expected '|'");
                    Read();
                    Current = new Token(TokenType.LogicalOr, position - tokenStart);
                    return true;
                }

                if (ch == '=')
                {
                    Read();
                    if (Peek() == '=')
                    {
                        Read();
                        Current = new Token(TokenType.Equals, position - tokenStart);
                    }
                    else
                        Current = new Token(TokenType.Assign, position - tokenStart);
                    return true;
                }

                if (ch == '!')
                {
                    Read();
                    if (Peek() != '=')
                        throw new Exception($"Unexpected symbol '{(char)Peek()}', expected '='");
                    Read();
                    Current = new Token(TokenType.NotEquals, position - tokenStart);
                    return true;
                }

                if (SingleCharTokens.ContainsKey(ch))
                {
                    Read();
                    Current = new Token(SingleCharTokens[ch], position - tokenStart);
                    return true;
                }

                throw new Exception($"Unexpected symbol '{ch}'");
            }
        }
    }
}
