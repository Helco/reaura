using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Aura.Script;

namespace Aura.Systems
{
    public class GlobalsSystem : BaseDisposable, IGameVariableSet, IVariableSet
    {
        private const string DefaultValueFile = "GlobalSettings.def";

        public string VariableSetName => "Global";

        private readonly IReadOnlyDictionary<string, int> Constants = new Dictionary<string, int>()
        {
            { "TEX_BIK", 0},
            { "FALSE", 0 },
            { "TRUE", 1 },
            { "FRONT", (int)CubeFace.Front }
        };
        private Regex NameRegex = new Regex(@"^%\w+$");

        private IReadOnlyDictionary<string, int> DefaultValues { get; }
        private Dictionary<string, int> values;

        public GlobalsSystem(IBackend backend)
        {
            using var defaultValuesStream = backend.OpenAssetFile(DefaultValueFile);
            if (defaultValuesStream == null)
                throw new FileNotFoundException($"Could not open {DefaultValueFile} for default values");
            using var streamReader = new StreamReader(defaultValuesStream);
            var scanner = new Tokenizer(DefaultValueFile, streamReader.ReadToEnd());
            var defaultValueNodes = new DefaultValueListParser(scanner).ParseDefaultValueList();

            var defaultValues = new Dictionary<string, int>();
            foreach (var node in defaultValueNodes.Values)
            {
                if (!NameRegex.IsMatch(node.Name))
                    throw new InvalidDataException($"{node.Position}: Invalid default value name \"{node.Name}\"");
                var name = node.Name.Substring(1);
                if (defaultValues.ContainsKey(name))
                    throw new InvalidDataException($"{node.Position}: Duplicate default value \"{name}\"");

                if (node.Value is NumericNode)
                    defaultValues.Add(name, (int)((NumericNode)node.Value).Value);
                else if (node.Value is StringNode)
                {
                    var constantName = ((StringNode)node.Value).Value;
                    if (!Constants.TryGetValue(constantName, out int constantValue))
                        throw new InvalidDataException($"{node.Position}: Unknown constant name \"{constantName}\"");
                    defaultValues.Add(name, constantValue);
                }
                else
                    throw new InvalidDataException($"{node.Value.Position}: Invalid default value \"{name}\"");
            }
            DefaultValues = defaultValues;

            values = defaultValues.ToDictionary(p => p.Key, p => p.Value);
        }

        public int this[string name]
        {
            get
            {
                if (!values.TryGetValue(name, out int value))
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unknown global variable \"{name}\"");
                return value;
            }
            set
            {
                if (!values.ContainsKey(name))
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unknown global variable \"{name}\"");
                values[name] = value;
            }
        }
    }
}
