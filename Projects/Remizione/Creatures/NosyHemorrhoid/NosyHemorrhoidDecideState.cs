using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// NosyHemorrhoidDecideState
    /// </summary>
    internal class NosyHemorrhoidDecideState : AIState
    {
        // Constructor
        public NosyHemorrhoidDecideState(AIStateMachine stateMachine)
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
