using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// EnviousEyeDecideState
    /// </summary>
    internal class EnviousEyeDecideState : AIState
    {
        // Constructor
        public EnviousEyeDecideState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Decide)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            StateMachine.ChangeState(AIStateName.Patrol);
        }
    }
}
