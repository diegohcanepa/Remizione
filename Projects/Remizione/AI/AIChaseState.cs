using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIChaseState
    /// </summary>
    public sealed class AIChaseState : AIState
    {
        private int startCooldown;

        // Constructor
        public AIChaseState(Actor owner)
            : base(owner)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            startCooldown = Randomizer.Next(400, 2000);
            Owner.FastMove = DiceBag.Dice10.Roll() < 4;
            Owner.MoveTowardsTarget();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (startCooldown > 0)
            {
                startCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            if (Owner.IsTargetInAttackRange())
                Signal = AIStateSignal.TargetInRange;

            //else if (!Owner.IsFollowingPath)
            //  signal = AIStateSignal.ChaseComplete;
            else
                Owner.MoveTowardsTarget();
        }

        // Exit
        public override void Exit()
        {
            Owner.FastMove = false;
        }
    }
}
