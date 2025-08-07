using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// AIStateMachine
    /// </summary>
    public sealed class AIStateMachine
    {
        private readonly Dictionary<AIStateName, AIState> states = [];

        // Constructor
        public AIStateMachine(Actor actor)
        {
            this.Actor = actor;
        }

        // Actor
        public Actor Actor { get; }

        // ChangeState
        public void ChangeState(AIStateName newState)
        {
            CurrentState?.Exit();
            CurrentState = states[newState];
            CurrentState?.Enter();
        }

        // CurrentState
        public AIState? CurrentState { get; private set; }

        // RegisterState
        public void RegisterState(AIState state) => states[state.Name] = state;

        // Update
        public void Update(GameTime gameTime)
        {
            CurrentState?.Update(gameTime);
        }
    }
}
