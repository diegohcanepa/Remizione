using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ZabulChargeState
    /// </summary>
    internal class ZabulChargeState : AIState
    {
        private int cooldown;

        // Constructor
        public ZabulChargeState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Charge)
        {
        }

        // MoveToEnemy
        private void MoveToEnemy()
        {
            if (Owner.FindEnemy() is GameThing enemy)
                Owner.MoveTo(enemy.Position);
        }

        // Enter
        public override void Enter()
        {
            cooldown = 0;
            MoveToEnemy();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (cooldown >= 0)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (cooldown <= 0)
                    MoveToEnemy();
                return;
            }

            if (!Owner.IsMoving)
                cooldown = 1000;
        }
    }
}
