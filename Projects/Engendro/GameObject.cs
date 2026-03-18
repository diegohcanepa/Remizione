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
            if (IsActiveInGameLoop)
                OnDraw(gameTime);
        }

        // Game
        public EngendroGame Game { get; } = game;

        // IsActiveInGameLoop
        public virtual bool IsActiveInGameLoop => true;

        // Update
        public void Update(GameTime gameTime)
        {
            if (IsActiveInGameLoop)
                OnUpdate(gameTime);
        }
    }
}
