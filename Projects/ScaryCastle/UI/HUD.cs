using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using ScaryCastle.Procedural;
using ScaryCastle.UI;

namespace ScaryCastle
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIHealthMeter healthMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.healthMeter = new(session.Game);
            this.Log = new(Game);
            this.Message = new(Game);

            // Sack slot
            this.SackSlot = new(session);

            // Coin meter 
            this.CoinMeter = new(Game);

            // Mini map
            this.MiniMap = new(Game);
        }

        #endregion

        #region Private members

        // DrawForActionMode
        private void DrawForActionMode(GameTime gameTime)
        {
            if (session.IsCurrentScene)
            {
                CoinMeter.Draw(gameTime);
                SackSlot.Draw(gameTime);
                Log.Draw(gameTime);
                Message.Draw(gameTime);

                if (session.Room is ProceduralRoom)
                    MiniMap.Draw(gameTime);
            }

            healthMeter.Draw(gameTime);
        }

        // DrawForAdventureMode
        private void DrawForAdventureMode(GameTime gameTime)
        {
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.GameplayMode == GameplayMode.Adventure)
                DrawForAdventureMode(gameTime);
            else
                DrawForActionMode(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.GameplayMode == GameplayMode.Action)
            {
                SackSlot.Update(gameTime);
                healthMeter.Update(gameTime);
                MiniMap.Update(gameTime);
                Log.Update(gameTime);
                Message.Update(gameTime);

                if (session.Player != null)
                {
                    CoinMeter.Value = session.Coins;
                    CoinMeter.Update(gameTime);
                }
            }
        }

        #endregion

        // CoinMeter
        public UICoinMeter CoinMeter { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            if (session.GameplayMode == GameplayMode.Action)
            {
                if (SackSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                    return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // Log
        public UILog Log { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            healthMeter.Actor = session.Player;
        }

        // SackSlot
        public UISackSlot SackSlot { get; }
    }
}
