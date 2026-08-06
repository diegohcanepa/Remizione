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

        private readonly HUDFaithMeter faithMeter;
        private readonly UIPassiveItems passiveItems;
        private readonly UIRunModifiers runModifiers;

        #endregion

        #region Constructor

        // Constructor
        public StatusHUD(GameSession session)
            : base(session)
        {
            this.faithMeter = new(session);
            this.StaminaMeter = new(session);
            this.HPMeter = new(session);
            this.InventoryMeter = new(session.PlayerInventory);
            this.passiveItems = new(session);
            this.PocketItems = new(session.PocketItemManager);
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
            StaminaMeter.Draw(gameTime);

            if (Session.IsCurrentScene)
            {
                InventoryMeter.Draw(gameTime);
                PocketItems.Draw(gameTime);
            }

            MiniMap.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            runModifiers.Update(gameTime);
            Statuses.Update(gameTime);
            passiveItems.Update(gameTime);
            PocketItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            HPMeter.Update(gameTime);
            faithMeter.Update(gameTime);
            StaminaMeter.Update(gameTime);
            MiniMap.Update(gameTime);
        }

        #endregion

        // HPMeter
        public UIHPMeter HPMeter { get; }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // PocketItems
        public UIPocketItems PocketItems { get; }

        // Reset
        public void Reset()
        {
            Statuses.Actor = Session.Player;
        }

        // StaminaMeter
        public HUDStaminaMeter StaminaMeter { get; }

        // Statuses
        public UIStatuses Statuses { get; }
    }
}