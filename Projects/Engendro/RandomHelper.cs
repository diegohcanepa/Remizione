using System;

namespace Engendro
{
    /// <summary>
    /// RandomHelper
    /// </summary>
    public static class RandomHelper
    {
        // GetSeed
        public static int GetSeed(int seed, int salt)
        {
            uint h = (uint)seed;

            h ^= (uint)salt * 0x9E3779B9; // golden number (Knuth)
            h ^= h >> 16;
            h *= 0x85EBCA6B;
            h ^= h >> 13;
            h *= 0xC2B2AE35;
            h ^= h >> 16;

            return (int)h;
        }

        // Next
        public static float Next(Random random, float minValue, float maxValue)
        {
            if (minValue == maxValue)
                return minValue;
            else
                return ((float)random.NextSingle() * (maxValue - minValue)) + minValue;
        }
    }
}
