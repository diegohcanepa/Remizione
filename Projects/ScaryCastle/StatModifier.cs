namespace ScaryCastle
{
    /// <summary>
    /// StatModifier
    /// </summary>
    public sealed class StatModifier
    {
        // Constructor
        public StatModifier(float value, object source)
        {
            this.Value = value;
            this.Source = source;
        }

        // Source
        public object Source { get; }

        // Value
        public float Value { get; }
    }
}
