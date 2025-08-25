using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// StagePlacementCondition
    /// </summary>
    public sealed class StagePlacementCondition : PlacementCondition
    {
        // Constructor
        public StagePlacementCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, Random random)
        {
            return Range.Contains(thing.Session.Stage);
        }

        // Range
        public Int32Range Range { get; }
    }
}
