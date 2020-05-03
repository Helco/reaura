using System;

namespace Aura.Script
{
    public struct ScriptPos
    {
        public string File { get; }
        public int Character { get; }
        public int Line { get; }
        public int Column { get; }
        public int Length { get; }

        public ScriptPos(string file, int character = 0, int line = 0, int column = 0, int length = 1)
        {
            File = file;
            Character = character;
            Line = line;
            Column = column;
            Length = length;
        }

        public ScriptPos(ScriptPos from, ScriptPos to)
        {
            if (from.File != to.File)
                throw new InvalidProgramException("Comparing ScriptPos from different files");
            File = from.File;
            Character = from.Character;
            Line = from.Line;
            Column = from.Column;
            Length = to.Character - from.Character;
        }

        public ScriptPos NextColumn => new ScriptPos(File, Character + 1, Line, Column + 1, Length);
        public ScriptPos NextLine => new ScriptPos(File, Character + 1, Line + 1, 0, Length);

        public static ScriptPos operator - (ScriptPos a, ScriptPos b) => new ScriptPos(a, b);

        public override string ToString()
        {
            string result = $"{File}:{Line + 1}:{Column + 1}";
            if (Length > 1)
                result += $":{Length}";
            return result;
        }
    }
}
