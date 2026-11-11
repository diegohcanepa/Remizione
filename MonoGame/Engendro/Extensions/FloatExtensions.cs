using System;

namespace Engendro
{
    /// <summary>
    /// FloatExtensions
    /// </summary>
    public static class FloatExtensions
    {
        extension(float value)
        {
            // IsBetween
            public bool IsBetween(float min, float max)
            {
                return value >= min && value <= max;
            }

            // IsOddNumber
            public bool IsOddNumber()
            {
                return value != 0 && value % 2 != 0;
            }

            // Round
            public float Round()
            {
                return (float)Math.Round(value, 0);
            }

            // Round
            public float Round(int decimals)
            {
                return (float)Math.Round(value, decimals);
            }
        }
    }
}
