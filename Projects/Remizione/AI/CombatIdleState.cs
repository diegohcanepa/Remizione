using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatIdleState
    /// </summary>
    public sealed class CombatIdleState : CombatState
    {
        // Constructor
        public CombatIdleState(CombatStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Actor.CanPerformAction)
            {
                Actor.SelectTarget();
                StateMachine.ExecuteAction(CombatStateSignal.Attack);
            }
        }
    }
}
