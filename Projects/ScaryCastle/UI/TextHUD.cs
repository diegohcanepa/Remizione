using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// TextHUD
    /// </summary>
    public sealed class TextHUD : GameObject, IInputHandler
    {
        private readonly GameSession session;

        #region Constructor

        // Constructor
        public TextHUD(GameSession session)
        {
            this.session = session;
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Giant);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            Log.Draw(gameTime);
            Message.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Log.Update(gameTime);
            Message.Update(gameTime);
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput()
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            return HandleInputResult.Unhandled;
        }

        // Log
        public UILog Log { get; } = new();

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            Message.Hide();
            Log.Hide();
        }
    }
}
