using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// WorldSizePlacementCondition
    /// </summary>
    public sealed class WorldSizePlacementCondition : PlacementCondition
    {
        // Constructor
        public WorldSizePlacementCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block, Random random)
        {
            return Range.Contains(block.Manager.Blocks.Count);
        }

        // Range
        public Int32Range Range { get; }
    }
}
