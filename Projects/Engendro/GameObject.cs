using Microsoft.Xna.Framework;

namespace Engendro
{
    /// <summary>
    /// GameObject
    /// </summary>
    public abstract class GameObject(EngendroGame game) : IDraw, IUpdate
    {
        #region Protected members

        // AfterDraw
        protected virtual void AfterDraw(GameTime gameTime)
        {
        }

        // BeforeDraw
        protected virtual void BeforeDraw(GameTime gameTime)
        {
        }

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
            {
                BeforeDraw(gameTime);
                OnDraw(gameTime);
                AfterDraw(gameTime);
            }
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
