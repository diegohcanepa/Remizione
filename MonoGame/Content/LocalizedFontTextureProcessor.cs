using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Content
{
    /// <summary>
    /// LocalizedFontTextureProcessor
    /// </summary>
    internal class LocalizedFontTextureProcessor : FontTextureProcessor
    {
        private readonly List<char> allCharacters = [];

        // DefaultCharacter
        public virtual char DefaultCharacter { get; init; }

        // Constructor
        public LocalizedFontTextureProcessor()
        {
            DefaultCharacter = '?';
            FirstCharacter = '\0';
            TextureFormat = TextureProcessorOutputFormat.Color;
        }

        /// <summary>
        /// Overridden to reference the list of characters.
        /// </summary>
        protected override char GetCharacterForIndex(int index) => allCharacters[index];

        // Process
        public override SpriteFontContent Process(Texture2DContent input, ContentProcessorContext context)
        {
            // Cast the input to our own special format
            if (input is not LocalizedFontTextureContent localizedContent)
                throw new InvalidOperationException();

            // Our localized texture input just contains the base Texture2DContent and the list of characters
            if (localizedContent.BaseContent is not Texture2DContent textureContent)
                throw new InvalidOperationException();

            allCharacters.AddRange(localizedContent.Chars);

            SpriteFontContent outputContent = base.Process(textureContent, context);

            //Need to set a default character for this kind of font as well
            //outputContent.DefaultCharacter = DefaultCharacter;

            return outputContent;
        }
    }
}
