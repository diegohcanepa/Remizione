using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// RunProgressPlacementCondition   
    /// </summary>
    public sealed class RunProgressPlacementCondition : PlacementCondition
    {
        // Constructor
        public RunProgressPlacementCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, Random random)
        {
            return Range.Contains(thing.Session.RunProgress);
        }

        // Range
        public Int32Range Range { get; }
    }
}
