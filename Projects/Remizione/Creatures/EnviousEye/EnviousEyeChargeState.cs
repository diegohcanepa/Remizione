using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// EnviousEyeChargeState
    /// </summary>
    internal class EnviousEyeChargeState : AIState
    {
        private int cooldown;

        // Constructor
        public EnviousEyeChargeState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Charge)
        {
        }

        // Enter
        public override void Enter()
        {
            cooldown = 0;

            if (Owner.PerceptionSensor.CurrentTarget is GameThing target)
            {
                Owner.FastMove = true;
                Owner.MoveTo(target.Position);
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
