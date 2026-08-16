using Adberration;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : SessionGameObject<GameSession>, IInputHandler
    {
        #region Private fields

        private readonly UIGooMeter gooMeter;
        private readonly UIPassiveItems passiveItems;
        private readonly UIRunModifiers runModifiers;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session)
        {
            this.gooMeter = new(session);
            this.StaminaMeter = new(session);
            this.HPMeter = new(session);
            this.InventoryMeter = new(session.PlayerInventory);
            this.passiveItems = new(session);
            this.PocketItems = new(session.PocketItemManager);
            this.MiniMap = new();
            this.runModifiers = new(session);
            this.Statuses = new(session);
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 14));
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
            gooMeter.Draw(gameTime);

            if (!Session.IsConsoleVisible)
                StaminaMeter.Draw(gameTime);

            if (Session.IsCurrentScene)
            {
                InventoryMeter.Draw(gameTime);
                PocketItems.Draw(gameTime);
            }

            MiniMap.Draw(gameTime);

            Message.Draw(gameTime);

            if (!Message.IsVisible)
                Log.Draw(gameTime);
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
            Log.Update(gameTime);
            Message.Update(gameTime);
            gooMeter.Update(gameTime);
            StaminaMeter.Update(gameTime);
            MiniMap.Update(gameTime);
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput()
        {
            if (Session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (Session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            return HandleInputResult.Unhandled;
        }

        // HPMeter
        public UIHPMeter HPMeter { get; }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Log
        public UILog Log { get; } = new();

        // Message
        public UIMessage Message { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // PocketItems
        public UIPocketItems PocketItems { get; }

        // Reset
        public void Reset()
        {
            Statuses.Actor = Session.Player;
            Message.Hide();
            Log.Hide();
        }

        // StaminaMeter
        public UIStaminaMeter StaminaMeter { get; }

        // Statuses
        public UIStatuses Statuses { get; }
    }
}