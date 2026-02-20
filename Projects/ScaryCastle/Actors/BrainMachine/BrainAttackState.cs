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

            if (Owner.GetTarget() is not { } target)
            {
                TransitionTo<BrainPatrolState>();
                return;
            }

            if (Brain.Decide(Owner, target) is not CombatIntent intent)
            {
                TransitionTo<CombatDecisionState>();
                return;
            }

            Owner.Attack(intent, target);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsAttacking)
                TransitionTo<CombatDecisionState>();
        }
    }
}