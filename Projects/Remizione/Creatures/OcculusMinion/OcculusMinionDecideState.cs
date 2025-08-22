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
            var enemy = Owner.FindEnemy();

            if (enemy == null)
                StateMachine.ChangeState(AIStateName.Patrol);

            else
                StateMachine.ChangeState(AIStateName.Charge);
        }
    }
}
