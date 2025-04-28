using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIAttackState
    /// </summary>
    public sealed class AIAttackState : AIState
    {
        private bool attackLaunched;

        // Constructor
        public AIAttackState(Actor owner)
            : base(owner)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            attackLaunched = false;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            if (Owner.Session.CombatManager.CurrentActor == Owner)
                Owner.Session.CombatManager.AdvanceTurn();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (attackLaunched)
            {
                if (!Owner.IsAttacking)
                    Signal = AIStateSignal.AttackComplete;
            }
            else
            {
                attackLaunched = true;
                Owner.CloseAttack(Owner.Target);
            }

            /*
            if (!Owner.IsTargetInAttackRange())
                Signal = AIStateSignal.TargetOutOfRange;

            else if (Owner.Target != null)
            {
                if (!attackDone)
                {
                    attackDone = true;
                    Owner.CloseAttack(Owner.Target);
                }
                else if (!Owner.IsAttacking)
                    Signal = AIStateSignal.TargetLost;
            }
            */
        }
    }
}
