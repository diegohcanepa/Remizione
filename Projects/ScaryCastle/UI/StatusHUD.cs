using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// StatusHUD
    /// </summary>
    public sealed class StatusHUD : SessionGameObject<GameSession>
    {
        #region Private fields

        private readonly UIPocketItemMeter bronzeKeyMeter;
        private readonly UIPocketItemMeter coinMeter;
        private readonly HUDFaithMeter faithMeter;
        private readonly UIPocketItemMeter goldenKeyMeter;
        private readonly UIPassiveItems passiveItems;
        private readonly UIRunModifiers runModifiers;
        private readonly HUDStaminaMeter staminaMeter;

        #endregion

        #region Constructor

        // Constructor
        public StatusHUD(GameSession session)
            : base(session)
        {
            this.faithMeter = new(session);
            this.staminaMeter = new(session);
            this.HPMeter = new(session);
            this.bronzeKeyMeter = new(session, PocketItemType.BronzeKey, new(7, -5), false);
            this.goldenKeyMeter = new(session, PocketItemType.GoldenKey, new(22, -5), true);
            this.coinMeter = new(session, PocketItemType.Coin, new(-7, -16), false);
            this.InventoryMeter = new(session.PlayerInventory);
            this.passiveItems = new(session);
            this.MiniMap = new();
            this.runModifiers = new(session);
            this.Statuses = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            HPMeter.Draw(gameTime);
            runModifiers.Draw(gameTime);
            Statuses.Draw(gameTime);
            passiveItems.Draw(gameTime);
            Game.SpriteBatch.End();

            faithMeter.Draw(gameTime);
            staminaMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            coinMeter.Draw(gameTime);
            bronzeKeyMeter.Draw(gameTime);
            goldenKeyMeter.Draw(gameTime);
            MiniMap.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            coinMeter.Update(gameTime);
            bronzeKeyMeter.Update(gameTime);
            runModifiers.Update(gameTime);
            Statuses.Update(gameTime);
            goldenKeyMeter.Update(gameTime);
            passiveItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            HPMeter.Update(gameTime);
            faithMeter.Update(gameTime);
            staminaMeter.Update(gameTime);
            MiniMap.Update(gameTime);
        }

        #endregion

        // HPMeter
        public UIHPMeter HPMeter { get; }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Reset
        public void Reset()
        {
            Statuses.Actor = Session.Player;
        }

        // Statuses
        public UIStatuses Statuses { get; }
    }
}