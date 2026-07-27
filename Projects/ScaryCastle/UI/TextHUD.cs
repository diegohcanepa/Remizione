using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using ScaryCastle.UI;

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
            this.BossMeter = new();
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 13));
            this.Sentence = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Message.Draw(gameTime);

            if (!Message.IsVisible)
                Log.Draw(gameTime);

            if (session.IsCurrentScene)
            {
                Sentence.Draw(gameTime);

                if (session.Room is ProceduralRoom rideRoom && rideRoom.RoomNode.Category == RoomCategory.Boss)
                    BossMeter.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            BossMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
            Sentence.Update(gameTime);
        }

        #endregion

        // BossMeter
        public UIBossMeter BossMeter { get; }

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
            BossMeter.Reset();
            Message.Hide();
            Log.Hide();
        }

        // Sentence
        public UISentence Sentence { get; }
    }
}
