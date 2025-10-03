using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// NosyHemorrhoidChargeState
    /// </summary>
    internal class NosyHemorrhoidChargeState : AIState
    {
        private int cooldown;

        // Constructor
        public NosyHemorrhoidChargeState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Charge)
        {
        }

        // Enter
        public override void Enter()
        {
            cooldown = 0;

            if (Owner.FindEnemy() is GameThing enemy)
            {
                Owner.FastMove = true;
                Owner.MoveTo(enemy.Position);
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
