namespace ScaryCastle.Battle
{
    /// <summary>
    /// Intent
    /// </summary>
    public sealed class Intent
    {
        // Constructor
        public Intent(IntentType intentType, int value)
        {
            this.IntentType = intentType;
            this.Value = value;
        }

        // IntentType
        public IntentType IntentType { get; }

        // Value
        public int Value { get; }
    }
}
