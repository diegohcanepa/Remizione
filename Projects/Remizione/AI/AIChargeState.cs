using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIChargeState
    /// </summary>
    public sealed class AIChargeState : AIState
    {
        // Constructor
        public AIChargeState(AIStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            Actor.MoveTowardsTarget();
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!Actor.IsMoving)
                StateMachine.ExecuteAction(AIStateSignal.CloseAttack);
        }
    }
}
