using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIAttackState
    /// </summary>
    public sealed class AIAttackState : AIState
    {
        private bool attackDone;

        // Constructor
        public AIAttackState(Actor owner)
            : base(owner)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            attackDone = false;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            //Owner.ReactionSpeedCooldown = Owner.Stats.ReactionSpeed;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (attackDone)
            {
                if (!Owner.IsAttacking)
                {
                    if (!Owner.IsTargetInAttackRange())
                        Signal = AIStateSignal.TargetOutOfRange;
                }
            }
            else
            {
                attackDone = true;
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
