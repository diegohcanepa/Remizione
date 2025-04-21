namespace Remizione
{
    /// <summary>
    /// ChancePlacementCondition
    /// </summary>
    public sealed class ChancePlacementCondition : PlacementCondition
    {
        // Constructor
        public ChancePlacementCondition(float chance)
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
