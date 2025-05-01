using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatCloseAttackState
    /// </summary>
    public sealed class CombatCloseAttackState : CombatState
    {
        private bool attackLaunched;

        // Constructor
        public CombatCloseAttackState(CombatStateMachine stateMachine)
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
