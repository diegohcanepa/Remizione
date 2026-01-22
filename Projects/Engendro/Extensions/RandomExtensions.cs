using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// RandomExtensions
    /// </summary>
    public static class RandomExtensions
    {
        extension(Random random)
        {
            // Next
            public float Next(float minValue, float maxValue)
            {
                return (float)((random.NextDouble() * (maxValue - minValue)) + minValue);
            }
        }
    }
}
