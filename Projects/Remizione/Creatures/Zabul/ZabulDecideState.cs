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
            var enemy = Owner.Session.Player;
            if (enemy != null)
                StateMachine.ChangeState(AIStateName.Charge);
        }
    }
}
