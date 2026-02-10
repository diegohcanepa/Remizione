using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorPrayState
    /// </summary>
    public sealed class ActorPrayState : ActorAnimatedState
    {
        // Constructor
        public ActorPrayState(Actor owner)
            : base(owner, ActorStateNames.Pray, false)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (Owner.Session.Will == 100)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }
    }
}
