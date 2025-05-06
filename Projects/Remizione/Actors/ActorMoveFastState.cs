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
            angerPenaltyCooldown = Owner.Stats.AngerPenaltyCooldown;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsCombating || !Owner.AngerMovePenalty)
                return;

            if (angerPenaltyCooldown > 0)
            {
                angerPenaltyCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                Owner.Anger -= Owner.Stats.AngerPenalty;
                angerPenaltyCooldown = Owner.Stats.AngerPenaltyCooldown;
            }
        }
    }
}
