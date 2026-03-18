using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// GameObject
    /// </summary>
    public abstract class GameObject
    {
        // Constructor
        protected GameObject()
        {
            this.Game = EngendroGame.Instance;
        }

        #region Protected members

        // OnDraw
        protected virtual void OnDraw(GameTime gameTime)
        {
        }

        // OnUpdate
        protected virtual void OnUpdate(GameTime gameTime)
        {
        }

        #endregion

        // Draw
        public void Draw(GameTime gameTime)
        {
            OnDraw(gameTime);
        }

        // Game
        public EngendroGame Game { get; }

        // Update
        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }
    }
}
