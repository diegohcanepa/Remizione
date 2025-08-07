using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatCloseAttackState
    /// </summary>
    public sealed class CombatCloseAttackState : AIState
    {
        private bool attackLaunched;

        // Constructor
        public CombatCloseAttackState(AIStateMachine stateMachine)
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
            }
            else
            {
                attackLaunched = true;
                Actor.PerformCloseAttack();
            }
        }
    }
}
