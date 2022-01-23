using System;
using System.IO;
using System.Text.RegularExpressions;
using Aura.Script;

namespace Aura
{
    public class Item : BaseDisposable
    {
        private static readonly Regex NameRegex = new Regex(@"^&\w+$");
        private IBackend backend;
        private readonly string spritePath, spriteActivePath, cursorSpritePath, cursorSpriteActivePath, animatePath;

        public ITexture Sprite => throw new NotImplementedException();
        public ITexture SpriteActive => throw new NotImplementedException();
        public ITexture CursorSprite => throw new NotImplementedException();
        public ITexture CursorSpriteActive => throw new NotImplementedException();
        public IVideoTexture Animate => throw new NotImplementedException();

        public string Name { get; }
        public string Description { get; }

        public Item(IBackend backend, ObjectNode objectNode)
        {
            string ExpectString(string name)
            {
                if (!objectNode.Properties.TryGetValue(name, out var propNode))
                    throw new InvalidDataException($"{objectNode.Position}: Missing property \"{name}\"");
                if (propNode.Value is not StringNode stringNode)
                    throw new InvalidDataException($"{propNode.Position}: Invalid value type for property \"{name}\"");
                return stringNode.Value;
            }

            if (!NameRegex.IsMatch(objectNode.Name))
                throw new InvalidDataException($"{objectNode.Position}: Invalid object name \"{objectNode.Name}\"");
            Name = objectNode.Name.Substring(1);
            Description = ExpectString("Description");
            spritePath = ExpectString("Sprite");
            spriteActivePath = ExpectString("SpriteActive");
            cursorSpritePath = ExpectString("CursorSprite");
            cursorSpriteActivePath = ExpectString("CursorSpriteActive");
            animatePath = ExpectString("Animate");
            this.backend = backend;
        }
    }
}
