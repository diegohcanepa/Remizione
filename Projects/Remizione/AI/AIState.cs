using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// AIState
    /// </summary>
    public abstract class AIState
    {
        // Constructor
        protected AIState(AIStateMachine stateMachine, AIStateName stateName)
        {
            this.StateMachine = stateMachine;
            this.Name = stateName;
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

        // StateMachine
        public AIStateMachine StateMachine { get; }

        // Name
        public AIStateName Name { get; }

        // Update
        public virtual void Update(GameTime gameTime)
        {
        }
    }
}
