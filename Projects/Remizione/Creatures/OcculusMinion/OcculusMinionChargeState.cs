using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// OcculusMinionChargeState
    /// </summary>
    internal class OcculusMinionChargeState : AIState
    {
        private int cooldown;

        // Constructor
        public OcculusMinionChargeState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Charge)
        {
        }

        // Enter
        public override void Enter()
        {
            cooldown = 0;

            if (Owner.LastKnownAttacker != null)
            {
                Owner.FastMove = true;
                Owner.MoveTo(Owner.LastKnownAttacker.Position);
                Owner.LastKnownAttacker = null;
            }
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (cooldown <= 0)
                    StateMachine.ChangeState(AIStateName.Decide);

                return;
            }

            if (!Owner.IsMoving)
                cooldown = 3000;
        }
    }
}
