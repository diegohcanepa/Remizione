using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// StatusHUD
    /// </summary>
    public sealed class StatusHUD : GameObject
    {
        #region Private fields

        private readonly UICoinMeter coinMeter;
        private readonly UIGooMeter gooMeter = new();
        private readonly UIHPMeter hpMeter;
        private readonly UIPassiveItems passiveItems;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public StatusHUD(GameSession session)
        {
            this.session = session;
            this.hpMeter = new(new(14, 3));
            this.coinMeter = new(session);
            this.InventoryMeter = new(session.PlayerInventory);
            this.passiveItems = new(session);
            this.BossMeter = new(session);
            this.ThingInfo = new();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            hpMeter.Draw(gameTime);
            passiveItems.Draw(gameTime);
            Game.SpriteBatch.End();

            gooMeter.Draw(gameTime);
            FaithMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            coinMeter.Draw(gameTime);
            ThingInfo.Draw(gameTime);

            if (session.IsCurrentScene)
            {
                if (session.Room is RideRoom rideRoom && rideRoom.RoomNode.RoomType == RoomType.Corridor)
                    BossMeter.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            coinMeter.Update(gameTime);
            passiveItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            BossMeter.Update(gameTime);
            ThingInfo.Update(gameTime);
            hpMeter.Update(gameTime);
            FaithMeter.Update(gameTime);
            gooMeter.Update(gameTime);
        }

        #endregion

        // BossMeter
        public UIBossMeter BossMeter { get; }

        // FaithMeter
        public UIFaithMeter FaithMeter { get; } = new();

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            gooMeter.Actor = session.Player;
            BossMeter.Reset();
            ThingInfo.Actor = null;
        }

        // ThingInfo
        public UIItemLoot ThingInfo { get; }
    }
}
