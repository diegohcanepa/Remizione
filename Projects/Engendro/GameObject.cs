using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// GameObject
    /// </summary>
    public abstract class GameObject(EngendroGame game)
    {
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
        public EngendroGame Game { get; } = game;

        // Update
        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);
        }
    }
}
