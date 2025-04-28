using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIChaseState
    /// </summary>
    public sealed class AIChaseState : AIState
    {
        // Constructor
        public AIChaseState(Actor owner)
            : base(owner)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            Owner.FastMove = true;
            Owner.MoveTowardsTarget();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Owner.IsMoving)
            {
                Signal = AIStateSignal.ChaseComplete;
                if (Owner.Target != null)
                    Owner.FaceTo(Owner.Target);
            }
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
