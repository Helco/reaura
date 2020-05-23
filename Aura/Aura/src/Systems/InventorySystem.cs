using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Aura.Script;

namespace Aura.Systems
{
    public class InventorySystem : BaseDisposable, IGameVariableSet
    {
        private static readonly Regex DescriptionRegex = new Regex(@"Description=(.+?);");
        private const string ItemListFile = "Scenes/Predmets/Predmets.prd";

        public string VariableSetName => "Predmet";

        public IReadOnlyDictionary<string, Item> AllItems { get; }
        public IEnumerable<Item> CurrentItems => currentItems;

        private HashSet<Item> currentItems = new HashSet<Item>();

        public InventorySystem(IBackend backend)
        {
            var encoding = System.Text.Encoding.GetEncoding("Latin1"); // TODO: Latin1 might only be correct for german
            using var stream = backend.OpenAssetFile(ItemListFile);
            if (stream == null)
                throw new FileNotFoundException($"Could not open item list file \"{ItemListFile}\"");
            using var streamReader = new StreamReader(stream, encoding);
            var objectListText = streamReader.ReadToEnd();
            objectListText = DescriptionRegex.Replace(objectListText, "Description=\"$1\";"); // easiest way for a very hacky file format
            var scanner = new Tokenizer(ItemListFile, objectListText);
            var objectList = new ObjectListParser(scanner).ParseObjectList();

            var allItems = new Dictionary<string, Item>();
            foreach (var objectNode in objectList)
            {
                var item = new Item(backend, objectNode);
                if (allItems.ContainsKey(item.Name))
                    throw new InvalidDataException($"{objectNode.Position}: Duplicate item definition \"{item.Name}\"");
                allItems.Add(item.Name, item);
            }
            AllItems = allItems;
        }

        protected override void DisposeManaged()
        {
            foreach (var item in AllItems.Values)
                item.Dispose();
        }

        public int this[string name]
        {
            get
            {
                if (!AllItems.TryGetValue(name, out var item))
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unknown item \"{name}\"");
                return CurrentItems.Contains(item) ? 1 : 0;
            }
            set
            {
                if (!AllItems.TryGetValue(name, out var item))
                    throw new ArgumentOutOfRangeException(nameof(name), $"Unknown item \"{name}\"");
                if (value == 0)
                    currentItems.Remove(item);
                else
                    currentItems.Add(item);
            }
        }
    }
}
