using Engendro;

namespace Remizione
{
    /// <summary>
    /// PlayerLevelCondition
    /// </summary>
    public sealed class PlayerLevelCondition : PlacementCondition
    {
        // Constructor
        public PlayerLevelCondition(Int32Range range)
        {
            this.Range = range;
        }

        // IsAvailable
        public override bool IsAvailable(GameThing thing, WorldBlock block)
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
