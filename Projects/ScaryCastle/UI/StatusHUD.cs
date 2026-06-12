using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            this.CoinMeter = new(session);
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
            PassiveItems.Draw(gameTime);
            Game.SpriteBatch.End();

            InventoryMeter.Draw(gameTime);
            CoinMeter.Draw(gameTime);

            if (session.IsCurrentScene)
            {
                if (session.Room is RideRoom rideRoom && rideRoom.RoomNode.RoomType == RoomType.Corridor)
                    BossMeter.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            CoinMeter.Update(gameTime);
            PassiveItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            BossMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            GooMeter.Update(gameTime);
        }

        #endregion

        // BossMeter
        public UIBossMeter BossMeter { get; }

        // CoinMeter
        public UICoinMeter CoinMeter { get; }

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
