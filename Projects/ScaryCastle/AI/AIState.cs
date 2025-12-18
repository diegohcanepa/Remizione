using Microsoft.Xna.Framework;

namespace ScaryCastle
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
            stateMachine.RegisterState(this);
        }

        // Enter
        public virtual void Enter()
        {
        }

        // Exit
        public virtual void Exit()
        {
        }

        // Owner
        public Actor Owner => StateMachine.Owner;

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
