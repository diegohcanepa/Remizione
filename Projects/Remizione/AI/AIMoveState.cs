using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIMoveState
    /// </summary>
    public sealed class AIMoveState : AIState
    {
        // Constructor
        public AIMoveState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Move)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            /*
            if (StateMachine.Destination.HasValue)
            {
                Actor.MoveTo(StateMachine.Destination.Value);
            }
            */
        }

        // Update
        public override void Update(GameTime gameTime)
        {
        }
    }
}
