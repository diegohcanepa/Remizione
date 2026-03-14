using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// BrainAttackState
    /// </summary>
    public class BrainAttackState : BrainState
    {
        // Enter
        public override void Enter()
        {
            Owner.StopMoving();

            Owner.ResetAttackTimer();

            if (Owner.Target == null)
            {
                TransitionTo<BrainPatrolState>();
                return;
            }

            if (Brain.Decide(Owner) is not CombatIntent intent)
            {
                TransitionTo<CombatDecisionState>();
                return;
            }

            Owner.Attack(intent, Owner.Target);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsAttacking)
                TransitionTo<CombatDecisionState>();
        }
    }
}