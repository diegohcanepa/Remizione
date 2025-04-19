using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// Vector3Extensions
    /// </summary>
    public static class Vector3Extensions
    {
        // Round
        public static Vector3 Round(this Vector3 value, int decimals)
        {
            value.X = value.X.Round(decimals);
            value.Y = value.Y.Round(decimals);
            value.Z = value.Z.Round(decimals);

            return value;
        }
    }
}
