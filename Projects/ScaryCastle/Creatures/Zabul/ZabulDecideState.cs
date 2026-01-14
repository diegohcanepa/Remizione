using Microsoft.Xna.Framework;

namespace ScaryCastle
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
        }
    }
}
