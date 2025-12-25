using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// Font
    /// </summary>
    public class Font
    {

        #region Protected members

        // OnSpriteFontChanged
        protected virtual void OnSpriteFontChanged()
        {
        }

        #endregion

        // GetMissingChars
        public static char[] GetMissingChars(SpriteFont spriteFont, IEnumerable<string> values)
        {
            List<char> result = [];

            var glyphDictionary = spriteFont.GetGlyphs();

            foreach (var value in values)
            {
                if (value != null)
                {
                    for (var i = 0; i < value.Length; i++)
                    {
                        if (!glyphDictionary.ContainsKey(value[i]))
                        {
                            if (!result.Contains(value[i]))
                            {
                                result.Add(value[i]);
                            }
                        }
                    }
                }
            }

            return [.. result];
        }

        // SpriteFont
        public SpriteFont? SpriteFont
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    OnSpriteFontChanged();
                }
            }
        }
    }
}
