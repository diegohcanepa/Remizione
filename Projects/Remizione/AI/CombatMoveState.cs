using EngendroAdventure;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatMoveState
    /// </summary>
    public sealed class CombatMoveState : CombatState
    {
        // Constructor
        public CombatMoveState(CombatStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            if (StateMachine.Destination.HasValue)
            {
                Actor.FastMove = true;
                Actor.MoveTo(StateMachine.Destination.Value);
            }
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Actor.Anger <= 0)
                StateMachine.ExecuteAction(CombatStateSignal.Fatigue);
            else if (!Actor.IsMoving)
                StateMachine.ExecuteAction(CombatStateSignal.EndTurn);
        }
    }
}
