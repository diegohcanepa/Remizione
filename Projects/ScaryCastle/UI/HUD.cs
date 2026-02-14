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

        private readonly ImageSprite playerIcon;
        private readonly UIHealthMeter healthMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.playerIcon = new(Game, Atlases.UI.GetImage("EdmundIcon"))
            {
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop),
            };

            this.healthMeter = new(session.Game, new(playerIcon.BoundingBox.Width, 2));
            this.Log = new(Game);
            this.Message = new(Game, RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Huge);

            // Coin meter 
            this.CoinMeter = new(session);

            // CommonInventoryMeter
            this.CommonInventoryMeter = new(session.CommonInventory);

            // SacredInventoryMeter
            this.SacredInventoryMeter = new(session.SacredInventory);

            // Mini map
            this.MiniMap = new(Game);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            playerIcon.Draw(gameTime);
            healthMeter.Draw(gameTime);

            CoinMeter.Draw(gameTime);
            CommonInventoryMeter.Draw(gameTime);
            SacredInventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            Message.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Room is ProceduralRoom)
                MiniMap.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            CommonInventoryMeter.Update(gameTime);
            SacredInventoryMeter.Update(gameTime);
            healthMeter.Update(gameTime);
            MiniMap.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
            CoinMeter.Update(gameTime);
        }

        #endregion

        // CoinMeter
        public UICoinMeter CoinMeter { get; }

        // CommonInventoryMeter
        public UIInventoryMeter CommonInventoryMeter { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

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

        // SacredInventoryMeter
        public UIInventoryMeter SacredInventoryMeter { get; }
    }
}
