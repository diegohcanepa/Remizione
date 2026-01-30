using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ArenaState
    /// </summary>
    public abstract class ArenaState
    {
        // Constructor
        protected ArenaState(Arena arena)
        {
            this.Arena = arena;
        }

        #region Protected members

        // Arena
        protected Arena Arena { get; }

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
            return HandleInputResult.Unhandled;
        }

        // Update
        public virtual void Update(GameTime gameTime)
        {
            TimeInState += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
