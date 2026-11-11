using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// PointExtensions
    /// </summary>
    public static class PointExtensions
    {
        extension(Point value)
        {
            // Distance
            public float Distance(Point value2)
            {
                float v1 = value.X - value2.X, v2 = value.Y - value2.Y;
                return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
            }
        }
    }
}
