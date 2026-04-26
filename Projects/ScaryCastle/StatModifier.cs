namespace ScaryCastle
{
    /// <summary>
    /// StatModifier
    /// </summary>
    public sealed class StatModifier(float value, object source)
    {
        // Source
        public object Source { get; } = source;

        // Value
        public float Value { get; } = value;
    }
}
