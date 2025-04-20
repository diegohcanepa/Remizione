namespace Remizione
{
    /// <summary>
    /// TagCondition
    /// </summary>
    public sealed class TagCondition : PlacementCondition
    {
        // Constructor
        public TagCondition(WorldBlockTag tag)
        {
            this.Tag = tag;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block)
        {
            return block.Tags.Contains(Tag);
        }

        // Tag
        public WorldBlockTag Tag { get; }
    }
}
