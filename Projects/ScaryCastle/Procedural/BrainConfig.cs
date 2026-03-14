namespace ScaryCastle
{
    /// <summary>
    /// BrainConfig
    /// </summary>
    public sealed class BrainConfig
    {
        // AttackCooldown
        public int AttackCooldown { get; set; } = 1500;

        // AttackRange
        public float AttackRange { get; set; } = 40;

        // DetectionRadius
        public float DetectionRadius { get; set; } = 25;

        // RageChargeTime
        public int RageChargeTime { get; set; } = 1000; // 0 = Inmediate, -1 = Passive
    }
}