namespace ScaryCastle
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        // Constructor
        public Environment()
        {
            this.DevilHand = new(DeityHandKind.Devil);
            this.GodHand = new(DeityHandKind.God);
        }

        #region Internal members

        // DevilHand
        internal DeityHand DevilHand { get; }

        // GodHand
        internal DeityHand GodHand { get; }

        #endregion
    }
}
