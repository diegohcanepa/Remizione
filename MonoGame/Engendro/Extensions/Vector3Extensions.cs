using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// Vector3Extensions
    /// </summary>
    public static class Vector3Extensions
    {
        extension(Vector3 value)
        {
            // Round
            public Vector3 Round(int decimals)
            {
                value.X = value.X.Round(decimals);
                value.Y = value.Y.Round(decimals);
                value.Z = value.Z.Round(decimals);

                return value;
            }
        }
    }
}
