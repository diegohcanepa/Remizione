using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorFatigueState
    /// </summary>
    public sealed class ActorFatigueState : ActorAnimatedState
    {
        // Constructor
        public ActorFatigueState(Actor owner)
            : base(owner, ActorStateNames.Fatigue, true)
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
