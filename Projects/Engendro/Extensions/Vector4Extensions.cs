using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// Vector4Extensions
    /// </summary>
    public static class Vector4Extensions
    {
        // Round
        public static Vector4 Round(this Vector4 value, int decimals)
        {
            value.X = value.X.Round(decimals);
            value.Y = value.Y.Round(decimals);
            value.Z = value.Z.Round(decimals);
            value.W = value.W.Round(decimals);

            return value;
        }
    }
}
