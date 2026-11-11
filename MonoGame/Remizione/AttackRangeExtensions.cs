namespace Remizione
{
    /// <summary>
    /// AttackRangeExtensions
    /// </summary>
    public static class AttackRangeExtensions
    {
        // GetHorzRange
        public static (float Min, float Max) GetHorzRange(this AttackRange range)
        {
            return range switch
            {
                AttackRange.Medium => (40f, 60f),
                AttackRange.Long => (70f, 120f),
                _ => (0, 0)
            };
        }
    }
}