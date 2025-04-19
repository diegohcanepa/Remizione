using System;

namespace Engendro
{
    /// <summary>
    /// FloatExtensions
    /// </summary>
    public static class FloatExtensions
    {
        // IsBetween
        public static bool IsBetween(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        // IsOddNumber
        public static bool IsOddNumber(this float value)
        {
            return value != 0 && value % 2 != 0;
        }

        // Round
        public static float Round(this float value)
        {
            return (float)Math.Round(value, 0);
        }

        // Round
        public static float Round(this float value, int decimals)
        {
            return (float)Math.Round(value, decimals);
        }
    }
}
