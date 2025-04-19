using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorMoveFastState
    /// </summary>
    public sealed class ActorMoveFastState : ActorState
    {
        private int staminaPenaltyCooldown;

        // Constructor
        public ActorMoveFastState(Actor owner)
            : base(owner, ActorStateNames.MoveFast, ActorStateSettings.LoopAnimation)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            staminaPenaltyCooldown = Owner.Stats.StaminaDegradationInterval;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (staminaPenaltyCooldown > 0)
            {
                staminaPenaltyCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                Owner.Stamina -= 1;
                staminaPenaltyCooldown = Owner.Stats.StaminaDegradationInterval;
            }
        }
    }
}
