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

        private readonly UIGraceMeter graceScore;
        private readonly UIHPMeter hpMeter;
        //private readonly UIRunModifiers runModifiers;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session)
        {
            this.hpMeter = new(session);
            this.InventoryMeter = new(session.PlayerData.Inventory);
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
            if (Session.DisplayHPMeter)
                hpMeter.Draw(gameTime);
            //runModifiers.Draw(gameTime);
            Statuses.Draw(gameTime);
            graceScore.Draw(gameTime);
            Game.SpriteBatch.End();

            if (Session.IsCurrentScene && Session.InventoryEnabled)
                InventoryMeter.Draw(gameTime);

            Message.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            //runModifiers.Update(gameTime);
            Statuses.Update(gameTime);
            InventoryMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            Message.Update(gameTime);
            graceScore.Update(gameTime);
            DestinationMark.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; } = new();

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

        // Message
        public UIMessage Message { get; }

        // Reset
        public void Reset()
        {
            Statuses.Actor = Session.Player;
            Message.Hide();
        }

        // Statuses
        public UIStatuses Statuses { get; }
    }
}