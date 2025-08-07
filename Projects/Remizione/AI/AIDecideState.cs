using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIDecideState
    /// </summary>
    public sealed class AIDecideState : AIState
    {
        // Constructor
        public AIDecideState(AIStateMachine stateMachine)
            : base(stateMachine, AIStateName.Decide)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            /*
            if (Actor.CanChangeState)
            {
                Actor.SelectTarget();
                Actor.FaceToTarget();
                StateMachine.ExecuteAction(AIStateSignal.Attack);
            }
            */
        }
    }
}
