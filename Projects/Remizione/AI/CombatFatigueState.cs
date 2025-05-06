using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// CombatFatigueState
    /// </summary>
    public sealed class CombatFatigueState : CombatState
    {
        private int cooldown;

        // Constructor
        public CombatFatigueState(CombatStateMachine stateMachine)
            : base(stateMachine)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            Actor.Fatigue();
            cooldown = 200;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (cooldown > 0)
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;

            else if (Actor.CanChangeState)
                StateMachine.ExecuteAction(CombatStateSignal.EndTurn);
        }
    }
}
