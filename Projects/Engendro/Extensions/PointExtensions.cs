using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// PointExtensions
    /// </summary>
    public static class PointExtensions
    {
        // Distance
        public static float Distance(this Point value, Point value2)
        {
            float v1 = value.X - value2.X, v2 = value.Y - value2.Y;
            return (float)Math.Sqrt((v1 * v1) + (v2 * v2));
        }
    }
}
