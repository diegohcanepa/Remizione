namespace ScaryCastle
{
    /// <summary>
    /// PlayerStats
    /// </summary>
    public sealed class PlayerStats
    {
        // AmbientLight
        public Stat AmbientLight { get; } = new(1);

        // Luck
        public Stat Luck { get; } = new(1);

        // RemoveAllModifiers
        public void RemoveAllModifiers(object source)
        {
            AmbientLight.RemoveModifiers(source);
            Luck.RemoveModifiers(source);
            Speed.RemoveModifiers(source);
        }

        // Speed
        public Stat Speed { get; } = new(1);
    }
}
