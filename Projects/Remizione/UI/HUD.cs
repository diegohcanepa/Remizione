using Adberration;
using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : SessionGameObject<GameSession>, IInputHandler
    {
        #region Private fields

        private readonly UIGraceMeter graceScore;
        private readonly UIMaterials materials;
        private readonly UIStatuses statuses;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session)
        {
            this.HeartMeter = new(session);
            this.InventoryMeter = new(session.PlayerData.Inventory);
            this.materials = new(session, new(25, 129.5f));
            this.DestinationMark = new(session);
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 10));
            this.graceScore = new(session);
            this.statuses = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            if (Session.DisplayHPMeter)
                HeartMeter.Draw(gameTime);
            graceScore.Draw(gameTime);
            statuses.Draw(gameTime);
            materials.Draw(gameTime);
            Game.SpriteBatch.End();

            if (Session.InventoryEnabled)
                InventoryMeter.Draw(gameTime);

            RoomTitle.Draw(gameTime);
            Message.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            HeartMeter.Update(gameTime);
            Message.Update(gameTime);
            graceScore.Update(gameTime);
            statuses.Update(gameTime);
            materials.Update(gameTime);
            DestinationMark.Update(gameTime);
            RoomTitle.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (Session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (Session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            return HandleInputResult.Unhandled;
        }

        // HeartMeter
        public UIHeartMeter HeartMeter { get; }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Message
        public UIMessage Message { get; }

        // Reset
        public void Reset()
        {
            Message.Hide();
            statuses.Actor = Session.Player;
        }

        // RoomTitle
        public UIRoomTitle RoomTitle { get; } = new();
    }
}