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

        private readonly HUDMessage actionMessage;
        private readonly UIFaithMeter faithMeter;
        private readonly UIHPMeter hpMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
        {
            this.session = session;

            this.actionMessage = new(RectanglePoint.Bottom, Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -5), ScaleInfo.Text.ExtraGiant);
            this.hpMeter = new(new(1, 0));
            this.faithMeter = new();
            this.Log = new();
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Huge);

            // CommonInventoryMeter
            this.InventoryMeter = new(session.Inventory);

            // Mini map
            this.MiniMap = new();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            hpMeter.Draw(gameTime);
            faithMeter.Draw(gameTime);

            InventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            actionMessage.Draw(gameTime);
            Message.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Room is ProceduralRoom)
                MiniMap.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            faithMeter.Update(gameTime);
            MiniMap.Update(gameTime);
            Log.Update(gameTime);
            actionMessage.Update(gameTime);
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

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Log
        public UILog Log { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Message
        public HUDMessage Message { get; }

        // NotifyCombatIntent
        public void NotifyCombatIntent(CombatIntent combatIntent)
        {
            actionMessage.Show(combatIntent.DisplayName, ColorPalette.Text.Red);
        }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            faithMeter.Actor = session.Player;
        }
    }
}
