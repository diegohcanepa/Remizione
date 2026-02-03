using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// CombatManagerState
    /// </summary>
    public abstract class CombatManagerState : GameObject, IInputHandler
    {
        // Constructor
        protected CombatManagerState(CombatManager manager)
            : base(manager.Session.Game)
        {
            this.Manager = manager;
        }

        #region Protected members

        // Manager
        protected CombatManager Manager { get; }

        // OnEnter
        protected virtual void OnEnter()
        {
        }

        // OnExit
        protected virtual void OnExit()
        {
        }

        // OnHandleInput
        protected virtual HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return HandleInputResult.Unhandled;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Manager.Session.IsCurrentScene)
                TimeInState += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // TimeInState
        protected float TimeInState { get; set; }

        #endregion

        // Enter
        public void Enter()
        {
            TimeInState = 0;
            OnEnter();
        }

        // Exit
        public void Exit()
        {
            OnExit();
        }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            return OnHandleInput(gameTime);
        }
    }
}
