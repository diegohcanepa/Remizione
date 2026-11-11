namespace Engendro
{
    /// <summary>
    /// IntExtensions
    /// </summary>
    public static class IntExtensions
    {
        extension(int value)
        {
            // IsBetween
            public bool IsBetween(int min, int max)
            {
                return value >= min && value <= max;
            }

            // IsOddNumber
            public bool IsOddNumber()
            {
                return value != 0 && value % 2 != 0;
            }
        }
    }
}
