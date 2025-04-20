namespace Remizione
{
    /// <summary>
    /// ChanceCondition
    /// </summary>
    public sealed class ChanceCondition : PlacementCondition
    {
        // Constructor
        public ChanceCondition(float chance)
        {
            this.Chance = chance;
        }

        // Chance (ratio)
        public float Chance { get; }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block)
        {
            return block.Session.Random.NextDouble() < Chance;
        }
    }
}
