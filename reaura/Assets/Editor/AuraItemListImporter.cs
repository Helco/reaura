using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using Aura.Script;
using UnityEngine.Video;

namespace Aura
{
    [ScriptedImporter(1, "prd")]
    public class AuraItemListImporter : ScriptedImporter
    {
        private Regex NameRegex = new Regex(@"^&\w+$");
        private Regex DescriptionRegex = new Regex(@"Description=(.+?);");

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var encoding = System.Text.Encoding.GetEncoding("Latin1"); // TODO: Latin1 might only be correct for german
            string itemListName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            var objectListText = File.ReadAllText(ctx.assetPath, encoding);
            objectListText = DescriptionRegex.Replace(objectListText, "Description=\"$1\";"); // easiest way for a very hacky file format
            var tokenizer = new Tokenizer(Path.GetFileName(ctx.assetPath), objectListText);
            var objects = new ObjectListParser(tokenizer).ParseObjectList();

            var items = new List<AuraSOItem>();
            foreach (var objectNode in objects)
                items.Add(ImportItem(ctx, objectNode));

            var list = ScriptableObject.CreateInstance<AuraSOItemList>();
            list.Items = items;
            list.name = itemListName;
            ctx.AddObjectToAsset(list.name, list);
            ctx.SetMainObject(list);
        }

        public AuraSOItem ImportItem(AssetImportContext ctx, ObjectNode node)
        {
            string directory = Path.GetDirectoryName(ctx.assetPath);

            T ExpectProperty<T>(string name) where T : ValueNode
            {
                if (!node.Properties.TryGetValue(name, out var prop))
                    throw new InvalidDataException($"{node.Position}: Expected the object property {name}");
                if (!(prop.Value is T))
                    throw new InvalidDataException($"{prop.Position}: Expected object property {name} to be {typeof(T).Name}");
                return prop.Value as T;
            }

            T ExpectAsset<T>(string name) where T : UnityEngine.Object
            {
                var pathNode = ExpectProperty<StringNode>(name);
                var path = Path.Combine(directory, pathNode.Value.Replace(".\\", "").Replace(".bik", ".webm"));
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null)
                    throw new InvalidDataException($"{pathNode.Position}: Could not find {typeof(T).Name} at {path}");
                return asset;
            }

            if (!NameRegex.IsMatch(node.Name))
                throw new InvalidDataException($"{node.Position}: Invalid object name");

            var item = ScriptableObject.CreateInstance<AuraSOItem>();
            item.name = node.Name.Substring(1);
            item.sprite = ExpectAsset<Texture2D>("Sprite");
            item.spriteActive = ExpectAsset<Texture2D>("SpriteActive");
            item.cursorSprite = ExpectAsset<Texture2D>("CursorSprite");
            item.cursorSpriteActive = ExpectAsset<Texture2D>("CursorSpriteActive");
            item.animate = ExpectAsset<VideoClip>("Animate");
            item.description = ExpectProperty<StringNode>("Description").Value;
            ctx.AddObjectToAsset(item.name, item);
            return item;
        }
    }
}
