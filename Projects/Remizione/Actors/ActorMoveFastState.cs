using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorMoveFastState
    /// </summary>
    public sealed class ActorMoveFastState : ActorAnimatedState
    {
        private int angerPenaltyCooldown;

        // Constructor
        public ActorMoveFastState(Actor owner)
            : base(owner, ActorStateNames.MoveFast, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            angerPenaltyCooldown = Owner.Stats.AngerDegradationInterval;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Session.CombatManager.TurnList.Count < 2)
                return;

            if (angerPenaltyCooldown > 0)
            {
                angerPenaltyCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                Owner.Anger -= 1;
                angerPenaltyCooldown = Owner.Stats.AngerDegradationInterval;
            }
        }
    }
}
