using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorMoveFastState
    /// </summary>
    public sealed class ActorMoveFastState : ActorAnimatedState
    {
        private int faithPenaltyCooldown;

        // Constructor
        public ActorMoveFastState(Actor owner)
            : base(owner, ActorStateNames.MoveFast, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            faithPenaltyCooldown = Owner.Stats.FaithPenaltyCooldown;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsCombating || !Owner.ShouldApplyMovePenalty)
                return;

            if (faithPenaltyCooldown > 0)
            {
                faithPenaltyCooldown -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                Owner.Faith -= Owner.Stats.MovePenalty;
                faithPenaltyCooldown = Owner.Stats.FaithPenaltyCooldown;
            }
        }
    }
}
