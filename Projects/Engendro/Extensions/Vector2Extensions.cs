using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Vector2Extensions
    /// </summary>
    public static class Vector2Extensions
    {
        private static readonly Random random = new();

        // AngleBetween
        public static double AngleBetween(this Vector2 vector1, Vector2 vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }

        // Random
        public static Vector2 Random(this Vector2 origin, float minimumRadius, float maximumRadius)
        {
            return Random(origin, new Vector2(minimumRadius), new Vector2(maximumRadius));
        }

        // Random
        public static Vector2 Random(this Vector2 origin, Vector2 minimumRadius, Vector2 maximumRadius)
        {
            if (minimumRadius.X < 0 || minimumRadius.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(minimumRadius), "Value must be greater than zero.");

            if (maximumRadius.X < 0 || maximumRadius.Y < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumRadius), "Value must be greater than zero.");

            if (minimumRadius.X > maximumRadius.X || minimumRadius.Y > maximumRadius.Y)
                throw new ArgumentOutOfRangeException(nameof(minimumRadius), "Minimum radius cannot be greater than the maximum radius.");

            var angle = random.NextDouble() * Math.PI * 2;
            var xRadius = RandomHelper.Next(System.Random.Shared, minimumRadius.X, maximumRadius.X);
            var yRadius = RandomHelper.Next(System.Random.Shared, minimumRadius.Y, maximumRadius.Y);
            var x = origin.X + xRadius * Math.Cos(angle);
            var y = origin.Y + yRadius * Math.Sin(angle);

            return new Vector2((float)x, (float)y);
        }

        // Round
        public static Vector2 Round(this Vector2 value, int decimals)
        {
            value.X = value.X.Round(decimals);
            value.Y = value.Y.Round(decimals);

            return value;
        }
    }
}
