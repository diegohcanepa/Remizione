using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ZabulDecideState
    /// </summary>
    internal class ZabulDecideState : AIState
    {
        // Constructor
        public ZabulDecideState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Decide)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.Session.IsAwaiting)
                return;

            if (Owner.PerceptionSensor.CurrentTarget is not GameThing target)
                return;

            if (target != null)
                StateMachine.ChangeState(AIStateName.Charge);
        }
    }
}
