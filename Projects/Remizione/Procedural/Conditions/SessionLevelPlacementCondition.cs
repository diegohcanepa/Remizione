using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// SessionLevelPlacementCondition
    /// </summary>
    public sealed class SessionLevelPlacementCondition : PlacementCondition
    {
        // Constructor
        public SessionLevelPlacementCondition(Int32Range range)
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
