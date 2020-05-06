using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using Aura.Script;

namespace Aura
{
    [ScriptedImporter(1, "def")]
    public class AuraVariableSetImporter : ScriptedImporter
    {
        private Regex NameRegex = new Regex(@"^%\w+$");

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var setName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            var tokenizer = new Tokenizer(Path.GetFileName(ctx.assetPath), File.ReadAllText(ctx.assetPath));
            var variables = new DefaultValueListParser(tokenizer).ParseDefaultValueList();

            var set = ScriptableObject.CreateInstance<AuraSOVariableSet>();
            set.name = setName;
            foreach (var variable in variables.Values)
            {
                if (!NameRegex.IsMatch(variable.Name))
                    throw new InvalidDataException($"{variable.Position}: Invalid variable name");

                int value;
                if (variable.Value is NumericNode)
                    value = (variable.Value as NumericNode).Value;
                else if (variable.Value is StringNode)
                    value = ParseConstant(variable.Value as StringNode);
                else
                    throw new InvalidDataException($"{variable.Position}: Invalid variable value");

                set[variable.Name.Substring(1)] = value;
            }

            ctx.AddObjectToAsset(setName, set);
            ctx.SetMainObject(set);
        }

        public static IReadOnlyDictionary<string, int> Constants = new Dictionary<string, int>()
        {
            { "FALSE", 0 },
            { "TRUE", 1 },
            { "TEX_BIK", -1 }
        };
        private int ParseConstant(StringNode v)
        {
            if (Constants.TryGetValue(v.Value, out int intValue))
                return intValue;
            return (int)AuraSceneScriptImporter.MapStringToCubemapFace(v);
        }
    }
}
