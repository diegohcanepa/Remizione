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

        private readonly HUDConditionMeter conditionMeter;
        private readonly UICoinMeter coinMeter;
        private readonly HUDGooMeter gooMeter;
        private readonly UIHPMeter hpMeter;
        private readonly UIPassiveItems passiveItems;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public StatusHUD(GameSession session)
        {
            this.session = session;
            this.conditionMeter = new(session);
            this.gooMeter = new(session);
            this.hpMeter = new(session);
            this.coinMeter = new(session);
            this.InventoryMeter = new(session.PlayerInventory);
            this.passiveItems = new(session);
            this.MiniMap = new();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            hpMeter.Draw(gameTime);
            passiveItems.Draw(gameTime);
            conditionMeter.Draw(gameTime);
            Game.SpriteBatch.End();

            gooMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            coinMeter.Draw(gameTime);
            MiniMap.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            coinMeter.Update(gameTime);
            passiveItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            gooMeter.Update(gameTime);
            conditionMeter.Update(gameTime);
            MiniMap.Update(gameTime);
        }

        #endregion

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Reset
        public void Reset()
        {
        }
    }
}
