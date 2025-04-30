using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AICloseAttackState
    /// </summary>
    public sealed class AICloseAttackState : AIState
    {
        private bool attackLaunched;

        // Constructor
        public AICloseAttackState(AIStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            attackLaunched = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (attackLaunched)
            {
                if (!Actor.IsAttacking)
                    StateMachine.EndTurn();
            }
            else
            {
                attackLaunched = true;
                Actor.CloseAttack();
            }
        }
    }
}
