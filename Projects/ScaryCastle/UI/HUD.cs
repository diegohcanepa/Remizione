using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIFaithMeter faithMeter;
        private readonly UIHPMeter hpMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
        {
            this.session = session;

            this.hpMeter = new(new(1, 0));
            this.faithMeter = new();
            this.Log = new();
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Huge);

            // CommonInventoryMeter
            this.InventoryMeter = new(session.Inventory);
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
            Message.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Room is RideRoom rideRoom && rideRoom.RoomNode.RoomType == RoomType.Corridor)
                BossMeter.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            BossMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            faithMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
        }

        #endregion

        // BossMeter
        public UIBossMeter BossMeter { get; } = new();

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

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            faithMeter.Actor = session.Player;
        }
    }
}
