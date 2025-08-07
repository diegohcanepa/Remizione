using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BloodyEyePatrolState
    /// </summary>
    public sealed class BloodyEyePatrolState : AIState
    {
        // Constructor
        public BloodyEyePatrolState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Patrol)
        {
        }

        // Enter
        public override void Enter()
        {
            Actor.MoveTo(Actor.Position - new Vector2(200, -20));
        }

        // Update
        public override void Update(GameTime gameTime)
        {
        }
    }
}
