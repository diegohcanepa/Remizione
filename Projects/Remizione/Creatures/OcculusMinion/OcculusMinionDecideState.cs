using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// OcculusMinionDecideState
    /// </summary>
    internal class OcculusMinionDecideState : AIState
    {
        // Constructor
        public OcculusMinionDecideState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Decide)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Owner.LastKnownAttacker == null)
                StateMachine.ChangeState(AIStateName.Patrol);
            else
                StateMachine.ChangeState(AIStateName.Charge);
        }
    }
}
