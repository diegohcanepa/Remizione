using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIIdleState
    /// </summary>
    public sealed class AIIdleState : AIState
    {
        // Constructor
        public AIIdleState(Actor owner)
            : base(owner)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            Owner.StopMoving();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.IsTargetInAttackRange())
                Signal = AIStateSignal.TargetInRange;
            else
                Signal = AIStateSignal.TargetOutOfRange;

            //if (Owner.CanSeeTarget())
            //{
            //    Signal = AIStateSignal.SawTarget;
            //    return;
            //}
        }
    }
}
