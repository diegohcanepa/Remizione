namespace ScaryCastle
{
    /// <summary>
    /// CombatDecision
    /// </summary>
    public sealed class CombatDecision(CombatDecisionType type, CombatIntent? intent, GameThing? target, PositioningMode positioningMode)
    {
        // Intent
        public CombatIntent? Intent { get; } = intent;

        // PositioningMode
        public PositioningMode PositioningMode { get; } = positioningMode;

        // Target
        public GameThing? Target { get; } = target;

        // Type
        public CombatDecisionType Type { get; } = type;
    }
}
