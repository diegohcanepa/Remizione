using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIPatrolState
    /// </summary>
    public sealed class AIPatrolState : AIState
    {
        // Constructor
        public AIPatrolState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Patrol)
        {
        }

        // Enter
        public override void Enter()
        {
            /*
            if (StateMachine.Destination.HasValue)
                Actor.MoveTo(StateMachine.Destination.Value);
            */
        }

        // Update
        public override void Update(GameTime gameTime)
        {
        }
    }
}
