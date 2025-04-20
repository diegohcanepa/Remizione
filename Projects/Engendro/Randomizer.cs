using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Randomizer
    /// </summary>
    public static class Randomizer
    {
        // Next
        public static T Next<T>() where T : struct
        {
            var values = Enum.GetNames(typeof(T));
            var index = Next(values);
            return Enum.Parse<T>(values[index]);
        }

        // Next
        public static int Next(Array array) => Next(0, array.Length - 1);

        // Next
        public static int Next(int maxValue) => Next(0, maxValue);

        // Next
        public static int Next(int minValue, int maxValue)
        {
            return minValue == maxValue ? minValue : Random.Next(minValue, maxValue == int.MaxValue ? maxValue : maxValue + 1);
        }

        // Next
        public static float Next(float maxValue) => Next(0f, maxValue);

        // Next
        public static float Next(float minValue, float maxValue)
        {
            if (minValue == maxValue)
                return minValue;
            else
                return (float)Random.NextDouble() * (maxValue - minValue) + minValue;
        }

        // Next
        public static long Next(long maxValue) => Next(0, maxValue);

        // Next
        public static long Next(long minValue, long maxValue)
        {
            if (minValue == maxValue)
                return minValue;
            else
                return Random.NextInt64() * (maxValue - minValue) + minValue;
        }

        // Next
        public static double Next(double maxValue) => Next(0, maxValue);

        // Next
        public static double Next(double minValue, double maxValue)
        {
            if (minValue == maxValue)
                return minValue;
            else
                return Random.NextDouble() * (maxValue - minValue) + minValue;
        }

        // Next
        public static Vector2 Next(Vector2 minValue, Vector2 maxValue)
        {
            return new(Next(minValue.X, maxValue.X), Next(minValue.Y, maxValue.Y));
        }

        // Random
        public static readonly Random Random = new();
    }
}
