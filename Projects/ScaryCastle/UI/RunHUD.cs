using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class RunHUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIGooMeter gooMeter;
        private readonly UIPassiveItems passiveItems;
        private readonly UIRunModifiers runModifiers;
        private readonly Run run;
        private readonly UITraits traits;

        #endregion

        #region Constructor

        // Constructor
        public RunHUD(Run run)
        {
            this.run = run;
            this.gooMeter = new(run.Session);
            this.StaminaMeter = new(run.Session);
            this.HPMeter = new(run.Session);
            this.InventoryMeter = new(run.PlayerInventory);
            this.passiveItems = new(run.Session);
            this.traits = new(run.Session);
            this.PocketItems = new(run.PocketItems);
            this.MiniMap = new();
            this.runModifiers = new(run);
            this.Statuses = new(run.Session);
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
            traits.Draw(gameTime);
            Game.SpriteBatch.End();

            gooMeter.Draw(gameTime);

            if (!run.Session.IsConsoleVisible)
                StaminaMeter.Draw(gameTime);

            if (run.Session.IsCurrentScene)
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
            traits.Update(gameTime);
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
            if (run.Session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (run.Session.IsConsoleVisible)
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
            Statuses.Actor = run.Session.Player;
            Message.Hide();
            Log.Hide();
        }

        // StaminaMeter
        public UIStaminaMeter StaminaMeter { get; }

        // Statuses
        public UIStatuses Statuses { get; }
    }
}