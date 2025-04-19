namespace Engendro
{
    /// <summary>
    /// IntExtensions
    /// </summary>
    public static class IntExtensions
    {
        // IsBetween
        public static bool IsBetween(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        // IsOddNumber
        public static bool IsOddNumber(this int value)
        {
            return value != 0 && value % 2 != 0;
        }
    }
}
