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
        public AIStateMachine(Actor owner)
        {
            this.Owner = owner;
        }

        // ChangeState
        public void ChangeState(AIStateName? newState)
        {
            CurrentState?.Exit();
            CurrentState = newState.HasValue ? states[newState.Value] : null;
            CurrentState?.Enter();
        }

        // CurrentState
        public AIState? CurrentState { get; private set; }

        // Owner
        public Actor Owner { get; }

        // RegisterState
        public void RegisterState(AIState state)
        {
            if (states.ContainsKey(state.Name))
                throw new System.Exception($"State {state.Name} is already registered in the AIStateMachine.");

            states[state.Name] = state;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            CurrentState?.Update(gameTime);
        }
    }
}
