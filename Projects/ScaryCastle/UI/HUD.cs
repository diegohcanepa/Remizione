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
        private readonly UIRunProgressMeter progressMeter;
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
            this.progressMeter = new(session);

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
            progressMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            Message.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.IsCurrentScene)
            {
                if (session.Room is RideRoom rideRoom && rideRoom.RoomNode.RoomType == RoomType.Corridor)
                    GuardMeter.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            GuardMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            faithMeter.Update(gameTime);
            progressMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
        }

        #endregion

        // GuardMeter
        public UIGuardMeter GuardMeter { get; } = new();

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
