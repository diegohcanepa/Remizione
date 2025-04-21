using System;

namespace Remizione
{
    /// <summary>
    /// BlockTagPlacementCondition
    /// </summary>
    public sealed class BlockTagPlacementCondition : PlacementCondition
    {
        // Constructor
        public BlockTagPlacementCondition(WorldBlockTag tag)
        {
            this.Tag = tag;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block, Random random)
        {
            return block.Tags.Contains(Tag);
        }

        // Tag
        public WorldBlockTag Tag { get; }
    }
}
