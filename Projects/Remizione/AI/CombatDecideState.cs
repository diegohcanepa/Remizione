using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatDecideState
    /// </summary>
    public sealed class CombatDecideState : AIState
    {
        // Constructor
        public CombatDecideState(AIStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Actor.CanChangeState)
            {
                Actor.SelectTarget();
                Actor.FaceToTarget();

                StateMachine.ExecuteAction(AIStateSignal.Attack);

                // Add other actions based on combat state
            }
        }
    }
}
