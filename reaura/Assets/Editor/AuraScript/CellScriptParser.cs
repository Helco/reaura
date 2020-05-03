using System;

namespace Aura.Script
{
    public class CellScriptParser : Parser
    {
        public CellScriptParser(Tokenizer tokenizer) : base(tokenizer) { }

        public InstructionBlockNode ParseCellScript()
        {
            var block = ParseInstructionBlock();
            Expect(TokenType.EndOfSource);
            return block;
        }
    }
}
