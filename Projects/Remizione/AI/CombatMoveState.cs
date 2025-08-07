using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatMoveState
    /// </summary>
    public sealed class CombatMoveState : AIState
    {
        // Constructor
        public CombatMoveState(AIStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            if (StateMachine.Destination.HasValue)
            {
                Actor.MoveTo(StateMachine.Destination.Value);
            }
        }

        // Update
        public override void Update(GameTime gameTime)
        {
        }
    }
}
