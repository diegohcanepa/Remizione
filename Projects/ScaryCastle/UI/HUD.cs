using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
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

            // Coin meter 
            this.CoinMeter = new(session);

            // Inventory
            this.Inventory = new(session);

            // Sack meter
            this.SackMeter = new(session);

            // Mini map
            this.MiniMap = new(Game);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.IsCurrentScene)
            {
                Inventory.Draw(gameTime);
                CoinMeter.Draw(gameTime);
                SackMeter.Draw(gameTime);
                Log.Draw(gameTime);
                Message.Draw(gameTime);

                if (session.Room is ProceduralRoom)
                    MiniMap.Draw(gameTime);
            }

            healthMeter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            Inventory.Update(gameTime);
            SackMeter.Update(gameTime);
            healthMeter.Update(gameTime);
            MiniMap.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
            CoinMeter.Update(gameTime);
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

            if (Inventory.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            return HandleInputResult.Unhandled;
        }

        // Inventory
        public UIInventory Inventory { get; }

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

        // SackMeter
        public UISackMeter SackMeter { get; }
    }
}
