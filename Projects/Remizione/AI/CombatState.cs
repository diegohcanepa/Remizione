using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIState
    /// </summary>
    public abstract class CombatState
    {
        // Constructor
        protected CombatState(CombatStateMachine stateMachine)
        {
            this.StateMachine = stateMachine;
        }

        // Actor
        public Actor Actor => StateMachine.Actor;
        
        // Enter
        public virtual void Enter()
        {
        }

        // Exit
        public virtual void Exit()
        {
        }

        // HandleSignal
        public virtual bool HandleSignal(CombatStateSignal signal) => false;

        // StateMachine
        public CombatStateMachine StateMachine { get; }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
