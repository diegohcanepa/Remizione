using Engendro.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// StateMachine
    /// </summary>
    public class StateMachine<TOwner> : IInputHandler
    {
        private bool isStarted;
        private readonly Dictionary<Type, State<TOwner>> states = [];

        // Constructor
        public StateMachine(TOwner owner, State<TOwner> initialState)
        {
            Owner = owner;
            InitializeAndAdd(initialState);
            CurrentState = initialState;
        }

        #region Private members

        // EnsureStarted
        private void EnsureStarted()
        {
            if (!isStarted)
                throw new InvalidOperationException("State machine not started.");
        }

        // InitializeAndAdd
        private void InitializeAndAdd(State<TOwner> state)
        {
            var type = state.GetType();
            if (!states.ContainsKey(type))
            {
                state.Initialize(this);
                states[type] = state;
            }
        }

        #endregion

        #region Protected members

        // OnStateChanged
        protected virtual void OnStateChanged()
        {
        }

        #endregion

        // AddState
        public void AddState(State<TOwner> state)
        {
            InitializeAndAdd(state);
        }

        // ChangeState
        public void ChangeState<TNextState>() where TNextState : State<TOwner>
        {
            ChangeState(typeof(TNextState));
        }

        // ChangeState
        public void ChangeState(Type nextStateType)
        {
            EnsureStarted();

            if (CurrentState.GetType() == nextStateType)
                return;

            // Verificamos si existe, si no, explotamos (fail fast)
            if (states.TryGetValue(nextStateType, out var nextState))
            {
                CurrentState.Exit();
                CurrentState = nextState;
                CurrentState.Enter();
                OnStateChanged();
            }
            else
            {
                throw new InvalidOperationException($"State {nextStateType.Name} not registered.");
            }
        }

        // CurrentState
        public State<TOwner> CurrentState { get; private set; }

        // FindState
        public TState? FindState<TState>() where TState : State<TOwner>
        {
            return FindState(typeof(TState)) as TState;
        }

        // FindState
        public State<TOwner>? FindState(Type stateType)
        {
            return states.TryGetValue(stateType, out var state) ? state : null;
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            return CurrentState.HandleInput(gameTime);
        }

        // Owner
        public TOwner Owner { get; }

        // Start
        public void Start()
        {
            if (isStarted)
                return;
            
            isStarted = true;
            CurrentState.Enter();
            OnStateChanged();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            CurrentState.Update(gameTime);
        }
    }
}