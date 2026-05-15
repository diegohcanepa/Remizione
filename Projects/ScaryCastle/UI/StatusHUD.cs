using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// StatusHUD
    /// </summary>
    public sealed class StatusHUD : GameObject
    {
        #region Private fields

        private readonly UIHPMeter hpMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public StatusHUD(GameSession session)
        {
            this.session = session;
            this.hpMeter = new(new(5, 3));
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
            Countdown.Update(gameTime);
        }

        #endregion

        // BossMeter
        public UIBossMeter BossMeter { get; }

        // GateMeter
        public UICountdown Countdown { get; }

        // GooMeter
        public UIGooMeter GooMeter { get; } = new();

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // PassiveItems
        public UIPassiveItems PassiveItems { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            GooMeter.Actor = session.Player;
            BossMeter.Reset();
        }
    }
}
