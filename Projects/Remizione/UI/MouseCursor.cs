using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// MouseCursor
    /// </summary>
    public sealed class MouseCursor : GameObject
    {
        #region Private fields

        private ImageSprite activeImage;
        private readonly ImageSprite defaultImage;
        private Vector2 position;
        private MouseCursorState state;
        private readonly ImageSprite waitImage;

        #endregion

        // Constructor
        public MouseCursor(EngendroGame game)
            : base(game)
        {
            if (Instance != null)
                throw new InvalidOperationException("This class cannot be instantiated twice.");
            else
                Instance = this;

            this.defaultImage = new ImageSprite(game);
            this.waitImage = new ImageSprite(game) { PivotOrigin = RectanglePoint.Middle };
            this.activeImage = defaultImage;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            activeImage.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            this.Position = InputManager.DefaultPlayer.Mouse.VirtualPosition;

            if (activeImage.Image == null)
            {
                if (state == MouseCursorState.Default)
                    activeImage.Image = Atlases.UI.MouseCursorDefault;

                else if (state == MouseCursorState.Wait)
                    activeImage.Image = Atlases.UI.MouseCursorWait;
            }

            activeImage.Update(gameTime);
        }

        #endregion

        // Instance
        public static MouseCursor Instance { get; private set; } = null!;

        // Position
        public Vector2 Position
        {
            get => position;
            set
            {
                this.position = value;
                activeImage.Position = value;
            }
        }

        // State
        public MouseCursorState State
        {
            get => state;
            set
            {
                if (value != state)
                {
                    state = value;

                    if (state == MouseCursorState.Default)
                        activeImage.Image = Atlases.UI.MouseCursorDefault;

                    else if (state == MouseCursorState.Wait)
                        activeImage.Image = Atlases.UI.MouseCursorWait;
                }
            }
        }
    }
}
