using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIChargeState
    /// </summary>
    public sealed class AIChargeState : AIState
    {
        private AIStateSignal signal;

        // Constructor
        public AIChargeState(Actor owner)
            : base(owner)
        {
        }

        // Enter
        public override void Enter()
        {
            signal = AIStateSignal.None;

            Owner.FastMove = true;

            if (Owner.Session.Player != null)
                Owner.MoveTo(Owner.Session.Player.Position);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsFollowingPath)
            {
                signal = AIStateSignal.ChargeComplete;
                return;
            }
        }

        // Exit
        public override void Exit()
        {
            Owner.FastMove = false;
        }

        // GetSignal
        public override AIStateSignal GetSignal() => signal;
    }
}
