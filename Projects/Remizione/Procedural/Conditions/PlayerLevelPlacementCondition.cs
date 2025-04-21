using Engendro;
using System;

namespace Remizione
{
    /// <summary>
    /// PlayerLevelPlacementCondition
    /// </summary>
    public sealed class PlayerLevelPlacementCondition : PlacementCondition
    {
        // Constructor
        public PlayerLevelPlacementCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block, Random random)
        {
            if (thing.Session.Player == null)
                return false;
            else
                return Range.Contains(thing.Session.Player.Level);
        }

        // Range
        public Int32Range Range { get; }
    }
}
