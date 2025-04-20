using Engendro;

namespace Remizione
{
    /// <summary>
    /// WorldSizeCondition
    /// </summary>
    public sealed class WorldSizeCondition : PlacementCondition
    {
        // Constructor
        public WorldSizeCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block)
        {
            return Range.Contains(block.Manager.Blocks.Count);
        }

        // Range
        public Int32Range Range { get; }
    }
}
