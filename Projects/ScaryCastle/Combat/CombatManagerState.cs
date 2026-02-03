using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// CombatManagerState
    /// </summary>
    public abstract class CombatManagerState : IInputHandler
    {
        // Constructor
        protected CombatManagerState(CombatManager manager)
        {
            this.Manager = manager;
        }

        #region Protected members

        // Manager
        protected CombatManager Manager { get; }

        // OnHandleInput
        protected virtual HandleInputResult OnHandleInput(GameTime gameTime)
        {
            return HandleInputResult.Unhandled;
        }

        // TimeInState
        protected float TimeInState { get; set; }

        #endregion

        // Enter
        public virtual void Enter()
        {
            TimeInState = 0;
        }

        // Exit
        public virtual void Exit()
        {
        }

        // HandleInput
        public virtual HandleInputResult HandleInput(GameTime gameTime)
        {
            return OnHandleInput(gameTime);
        }

        // Update
        public virtual void Update(GameTime gameTime)
        {
            if (Manager.Session.IsCurrentScene)
                TimeInState += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
