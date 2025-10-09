using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// CompletedRunsPlacementCondition   
    /// </summary>
    public sealed class CompletedRunsPlacementCondition : PlacementCondition
    {
        // Constructor
        public CompletedRunsPlacementCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(ProceduralRoom room, GameThing thing, Random random)
        {
            return Range.Contains(thing.Session.RunProgress);
        }

        // Range
        public Int32Range Range { get; }
    }
}
