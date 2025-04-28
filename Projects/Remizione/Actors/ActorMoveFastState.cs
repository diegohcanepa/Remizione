using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorMoveFastState
    /// </summary>
    public sealed class ActorMoveFastState : ActorState
    {
        private int willpowerPenaltyCooldown;

        // Constructor
        public ActorMoveFastState(Actor owner)
            : base(owner, ActorStateNames.MoveFast, ActorStateSettings.LoopAnimation)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            willpowerPenaltyCooldown = Owner.Stats.WillpowerDegradationInterval;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (willpowerPenaltyCooldown > 0)
            {
                willpowerPenaltyCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                Owner.Willpower -= 1;
                willpowerPenaltyCooldown = Owner.Stats.WillpowerDegradationInterval;
            }
        }
    }
}
