using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIIdleState
    /// </summary>
    public sealed class AIIdleState : AIState
    {
        private int cooldown;

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
            cooldown = 500;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (cooldown > 0)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            if (Owner.IsTargetInAttackRange())
                Signal = AIStateSignal.TargetInRange;
            else
                Signal = AIStateSignal.TargetOutOfRange;

            /*
            if (Owner.IsTargetInAttackRange())
                Signal = AIStateSignal.TargetInRange;
            else
                Signal = AIStateSignal.TargetOutOfRange;

            //if (Owner.CanSeeTarget())
            //{
            //    Signal = AIStateSignal.SawTarget;
            //    return;
            //}
            */
        }
    }
}
