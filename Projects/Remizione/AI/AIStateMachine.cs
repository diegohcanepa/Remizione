using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// AIStateMachine
    /// </summary>
    public class AIStateMachine
    {
        private AIState currentState;
        private readonly Dictionary<(Type, AIStateSignal), AIState> transitions = [];

        // Constructor
        public AIStateMachine(Actor owner, AIState initialState)
        {
            this.Owner = owner;
            currentState = initialState;
            currentState.Enter();
        }

        // AddTransition
        public void AddTransition<TState>(AIStateSignal signal, AIState targetState)
        {
            transitions[(typeof(TState), signal)] = targetState;
        }

        // Owner
        public Actor Owner { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            currentState.Update(gameTime);

            var signal = currentState.GetSignal();
            if (signal != AIStateSignal.None && transitions.TryGetValue((currentState.GetType(), signal), out var newState))
            {
                currentState.Exit();
                currentState = newState;
                currentState.Enter();
            }
        }
    }
}