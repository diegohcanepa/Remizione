using Microsoft.Xna.Framework;

namespace Remizione
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
            var target = Owner.PerceptionSensor.CurrentTarget;

            if (target == null)
                StateMachine.ChangeState(AIStateName.Patrol);
            else
                StateMachine.ChangeState(AIStateName.Charge);
        }
    }
}
