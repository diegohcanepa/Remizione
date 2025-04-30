using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIState
    /// </summary>
    public abstract class AIState
    {
        // Constructor
        protected AIState(AIStateMachine stateMachine)
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
        public virtual bool HandleSignal(AIStateSignal signal) => false;

        // StateMachine
        public AIStateMachine StateMachine { get; }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
