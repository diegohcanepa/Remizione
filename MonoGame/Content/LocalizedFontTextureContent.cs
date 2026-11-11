using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Content
{
    /// <summary>
    /// Class to allow us to add the source path to the processor.
    /// </summary>
    public class LocalizedFontTextureContent(TextureContent baseContent, List<char> chars) : Texture2DContent
    {
        // BaseContent
        public TextureContent BaseContent { get; } = baseContent;

        // Chars
        public List<char> Chars { get; } = chars;
    }
}
