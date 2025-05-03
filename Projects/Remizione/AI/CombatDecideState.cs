using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatDecideState
    /// </summary>
    public sealed class CombatDecideState : CombatState
    {
        // Constructor
        public CombatDecideState(CombatStateMachine stateMachine)
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

                StateMachine.ExecuteAction(CombatStateSignal.Attack);

                // Add other actions based on combat state
            }
        }
    }
}
