namespace ScaryCastle
{
    /// <summary>
    /// PlayerStats
    /// </summary>
    public sealed class PlayerStats
    {
        // Luck
        public Stat Luck { get; } = new(1);

        // Speed
        public Stat Speed { get; } = new(1);
    }
}
