using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// StateMachine
    /// </summary>
    public class StateMachine<TOwner, TState> where TState : State<TOwner>
    {
        private readonly Dictionary<string, TState> states = [];

        // Constructor
        public StateMachine(TOwner owner, TState initialState)
        {
            Owner = owner;
            RegisterState(initialState);
            CurrentState = initialState;
        }

        #region Protected members

        // OnStateChanged
        protected virtual void OnStateChanged()
        {
        }

        #endregion

        // ChangeState
        public bool ChangeState(string newStateName, bool forceRestart = false)
        {
            if (newStateName == CurrentState.Name && !forceRestart)
                return false;

            if (states.TryGetValue(newStateName, out var value))
            {
                CurrentState.Exit();
                CurrentState = value;
                CurrentState.Enter();
                OnStateChanged();
                return true;
            }

            return false;
        }

        // CurrentState
        public TState CurrentState { get; private set; }

        // FindState
        public TState? FindState(string name)
        {
            return states.TryGetValue(name, out TState? state) ? state : null;
        }

        // Owner
        public TOwner Owner { get; }

        // RegisterState
        public void RegisterState(TState state)
        {
            RegisterState(state, false);
        }

        // RegisterState
        public void RegisterState(TState state, bool replaceExisting)
        {
            if (!replaceExisting && states.ContainsKey(state.Name))
                throw new InvalidOperationException("Duplicated state name.");

            states[state.Name] = state;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            var nextStateName = CurrentState.CheckTransitions();
            if (nextStateName != null && states.ContainsKey(nextStateName))
                ChangeState(nextStateName);

            CurrentState.Update(gameTime);
        }
    }
}
