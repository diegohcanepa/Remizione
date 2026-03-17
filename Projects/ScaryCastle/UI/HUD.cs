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

        private readonly UIHPMeter hpMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.ActionMessage = new(Game, RectanglePoint.Bottom, Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -5), ScaleInfo.Text.ExtraGiant);
            this.FearMeter = new(session);
            this.hpMeter = new(session.Game, new(1, 0));
            this.Log = new(Game);
            this.Message = new(Game, RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Huge);

            // CommonInventoryMeter
            this.InventoryMeter = new(session.Inventory);

            // Mini map
            this.MiniMap = new(Game);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            hpMeter.Draw(gameTime);

            if (session.IsCurrentScene)
                FearMeter.Draw(gameTime);

            InventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            ActionMessage.Draw(gameTime);
            Message.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Room is ProceduralRoom)
                MiniMap.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            FearMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            MiniMap.Update(gameTime);
            Log.Update(gameTime);
            ActionMessage.Update(gameTime);
            Message.Update(gameTime);
        }

        #endregion

        // ActionMessage
        public HUDMessage ActionMessage { get; }

        // FearMeter
        public UIFearMeter FearMeter { get; }

        // HandleInput
        public HandleInputResult HandleInput()
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            return HandleInputResult.Unhandled;
        }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Log
        public UILog Log { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
        }
    }
}
