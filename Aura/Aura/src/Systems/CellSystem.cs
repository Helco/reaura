using Aura.Script;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Aura.Systems
{
    public class Cell
    {
        public string Name { get; }
        public bool IsActive { get; set; }
        public Vector2 UpperLeft { get; }
        public Vector2 LowerRight => UpperLeft + Size;
        public Vector2 Size { get; }
        public InstructionBlockNode Action { get; }

        public Cell(string name, Vector2 upperLeft, Vector2 size, InstructionBlockNode action)
        {
            IsActive = true;
            Name = name;
            UpperLeft = upperLeft;
            Size = size;
            Action = action;
        }

        public bool IsPointInside(Vector2 auraPos)
        {
            bool wrappedIntervalInside(float val, float min, float max)
            {
                if (min < max)
                    return val >= min && val <= max;
                else
                    return val >= min || val <= max;
            }
            auraPos = AuraMath.NormalizeAura(auraPos);
            return
                wrappedIntervalInside(auraPos.X, UpperLeft.X, LowerRight.X) &&
                wrappedIntervalInside(auraPos.Y, UpperLeft.Y, LowerRight.Y);
        }
    }

    public class CellSystem : BaseDisposable, IObjectListSystem, IWorldInputHandler, IGameVariableSet
    {
        public string ObjectListName => "&Cells";
        public string VariableSetName => "Cell";

        private Interpreter? interpreter;
        private Dictionary<string, Cell> cells = new Dictionary<string, Cell>();

        public IEnumerable<Cell> Cells => cells.Values;

        public int this[string name]
        {
            get
            {
                if (!cells.TryGetValue(name, out var cell))
                    throw new ArgumentOutOfRangeException($"Unknown cell name {name}");
                return cell.IsActive ? 1 : 0;
            }
            set
            {
                if (!cells.TryGetValue(name, out var cell))
                    throw new ArgumentOutOfRangeException($"Unknown cell name {name}");
                cell.IsActive = value != 0;
            }
        }

        public void OnBeforeSceneChange(LoadSceneContext _)
        {
            cells.Clear();
        }

        public void AddObject(LoadSceneContext context, ObjectNode objectNode)
        {
            T ExpectProperty<T>(string name) where T : ValueNode
            {
                if (!objectNode.Properties.TryGetValue(name, out var cellProp))
                    throw new InvalidDataException($"{objectNode.Position}: Expected the cell property {name}");
                if (!(cellProp.Value is T))
                    throw new InvalidDataException($"{cellProp.Position}: Expected cell property {name} to be {typeof(T).Name}");
                return (T)cellProp.Value;
            }

            if (cells.ContainsKey(objectNode.Name))
                throw new InvalidDataException($"{objectNode.Position}: Cell {objectNode.Name} is defined twice");
            var posNode = ExpectProperty<VectorNode>("Pos");
            var sizeNode = ExpectProperty<VectorNode>("CellSize");
            var scriptNode = ExpectProperty<StringNode>("script");

            if (!context.ScriptTexts.TryGetValue(scriptNode.Value.Replace(".\\", ""), out var scriptText))
                throw new InvalidDataException($"{scriptNode.Position}: Could not find cell script {scriptNode.Value}");
            var scanner = new Tokenizer(scriptNode.Value, scriptText);
            var action = new CellScriptParser(scanner).ParseCellScript();

            cells[objectNode.Name] = new Cell(
                objectNode.Name,
                new Vector2(posNode.X, posNode.Y),
                new Vector2(sizeNode.X, sizeNode.Y),
                action);
        }

        public void OnWorldClick(Vector2 pos)
        {
            // let's find some weird places in Aura with Single
            var cell = cells.Values.SingleOrDefault(c => c.IsPointInside(pos));
            if (cell == null || interpreter == null)
                return;
            interpreter.Execute(cell.Action);
        }

        public void RegisterGameFunctions(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }
    }
}
