using System;

namespace Remizione
{
    /// <summary>
    /// RoomSizePlacementCondition
    /// </summary>
    public sealed class RoomSizePlacementCondition : PlacementCondition
    {
        // Constructor
        public RoomSizePlacementCondition(float chance)
        {
            this.Chance = chance;
        }

        // Chance (ratio)
        public float Chance { get; }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, Random random)
        {
            return random.NextDouble() < Chance;
        }
    }
}
