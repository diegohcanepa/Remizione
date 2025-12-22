using Microsoft.Xna.Framework.Graphics;

namespace Engendro
{
    /// <summary>
    /// SpriteFontExtensions
    /// </summary>
    public static class SpriteFontExtensions
    {
        extension(SpriteFont font)
        {
            // GetGlyphIndex
            public int GetGlyphIndex(char character)
            {
                for (var i = 0; i < font.Glyphs.Length; i++)
                {
                    if (font.Glyphs[i].Character == character)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
    }
}
