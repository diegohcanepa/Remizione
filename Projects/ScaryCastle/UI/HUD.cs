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

        private readonly UIHPMeter hpMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
        {
            this.session = session;
            this.hpMeter = new(new(5, 3));
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Giant);
            this.Countdown = new(session);
            this.InventoryMeter = new(session.PlayerInventory);
            this.PassiveItems = new(session);
            this.BossMeter = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            hpMeter.Draw(gameTime);
            GooMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            Message.Draw(gameTime);
            Countdown.Draw(gameTime);
            PassiveItems.Draw(gameTime);

            Game.SpriteBatch.End();

            if (session.IsCurrentScene)
            {
                if (session.Room is RideRoom rideRoom && rideRoom.RoomNode.RoomType == RoomType.Corridor)
                    BossMeter.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            PassiveItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            BossMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            GooMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
            Countdown.Update(gameTime);
        }

        #endregion

        // BossMeter
        public UIBossMeter BossMeter { get; }

        // GateMeter
        public UICountdown Countdown { get; }

        // GooMeter
        public UIGooMeter GooMeter { get; } = new();

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
        public UILog Log { get; } = new();

        // Message
        public HUDMessage Message { get; }

        // PassiveItems
        public UIPassiveItems PassiveItems { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            GooMeter.Actor = session.Player;
            Message.Hide();
            Log.Hide();
            BossMeter.Reset();
        }
    }
}
