using Adberration;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : SessionGameObject<GameSession>, IInputHandler
    {
        #region Private fields

        private readonly UIStatMeter energyMeter;
        private readonly UIGraceMeter graceScore;
        private readonly UIStatMeter hpMeter;
        //private readonly UIRunModifiers runModifiers;
        private readonly UITraits traits;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session)
        {
            this.hpMeter = new(session, StatName.HP, new(5, 3));
            this.energyMeter = new(session, StatName.Energy, new(5, 12));
            this.InventoryMeter = new(session.PlayerData.Inventory);
            this.traits = new(session);
            this.MiniMap = new();
            //this.runModifiers = new(run);
            this.Statuses = new(session);
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 14));

            // Grace
            this.graceScore = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            //runModifiers.Draw(gameTime);
            Statuses.Draw(gameTime);
            traits.Draw(gameTime);
            graceScore.Draw(gameTime);
            Game.SpriteBatch.End();

            if (Session.DisplayHPMeter)
                hpMeter.Draw(gameTime);

            if (Session.DisplayEnergyMeter)
                energyMeter.Draw(gameTime);

            if (Session.IsCurrentScene && Session.InventoryEnabled)
                InventoryMeter.Draw(gameTime);

            MiniMap.Draw(gameTime);

            Message.Draw(gameTime);

            if (!Message.IsVisible)
                Log.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            traits.Update(gameTime);
            //runModifiers.Update(gameTime);
            Statuses.Update(gameTime);
            InventoryMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
            energyMeter.Update(gameTime);
            MiniMap.Update(gameTime);
            graceScore.Update(gameTime);
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

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Log
        public UILog Log { get; } = new();

        // Message
        public UIMessage Message { get; }

        // MiniMap
        public UIMiniMap MiniMap { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = Session.Player;
            energyMeter.Actor = Session.Player;
            Statuses.Actor = Session.Player;
            Message.Hide();
            Log.Hide();
        }

        // Statuses
        public UIStatuses Statuses { get; }
    }
}