using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatCloseAttackState
    /// </summary>
    public sealed class AICloseAttackState : AIState
    {
        private bool attackLaunched;

        // Constructor
        public AICloseAttackState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.CloseAttack)
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
            }
            else
            {
                attackLaunched = true;
                Actor.PerformCloseAttack();
            }
        }
    }
}
