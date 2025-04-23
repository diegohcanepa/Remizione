using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// WorldCyclesPlacementCondition
    /// </summary>
    public sealed class WorldCyclesPlacementCondition : PlacementCondition
    {
        // Constructor
        public WorldCyclesPlacementCondition(Int32Range cycles)
        {
            this.Cycles = cycles;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block, Random random)
        {
            return Cycles.Contains(block.Manager.Blocks.Count);
        }

        // Cycles
        public Int32Range Cycles { get; }
    }
}
