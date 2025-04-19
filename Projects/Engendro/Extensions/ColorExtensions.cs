using Microsoft.Xna.Framework;
using System.Globalization;

namespace Engendro
{
    /// <summary>
    /// ColorExtensions
    /// </summary>
    public static class ColorExtensions
    {
        // FromHex
        public static Color FromHex(string value)
        {
            var r = int.Parse(value.Substring(1, 2), NumberStyles.HexNumber);
            var g = int.Parse(value.Substring(3, 2), NumberStyles.HexNumber);
            var b = int.Parse(value.Substring(5, 2), NumberStyles.HexNumber);
            var a = value.Length > 7 ? int.Parse(value.Substring(7, 2), NumberStyles.HexNumber) : 255;

            return new Color(r, g, b, a);
        }

        // Invert
        public static Color Invert(this Color color)
        {
            return new(255 - color.R, 255 - color.G, 255 - color.B, color.A);
        }
    }
}
