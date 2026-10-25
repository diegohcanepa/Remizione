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
        private readonly UIStatuses statuses;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session)
        {
            this.HeartMeter = new(session);
            this.InventoryMeter = new(session.PlayerData.Inventory);
            this.WoodMeter = new(session, Material.Wood, new(25, 131));
            this.StoneMeter = new(session, Material.Stone, new(37, 131));
            this.GutMeter = new(session, Material.Gut, new(49, 131));

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
            Game.SpriteBatch.End();

            if (Session.InventoryEnabled)
                InventoryMeter.Draw(gameTime);

            if (Session.MaterialsEnabled)
            {
                WoodMeter.Draw(gameTime);
                StoneMeter.Draw(gameTime);
                GutMeter.Draw(gameTime);
            }

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
            WoodMeter.Update(gameTime);
            StoneMeter.Update(gameTime);
            GutMeter.Update(gameTime);
            DestinationMark.Update(gameTime);
            RoomTitle.Update(gameTime);
        }

        #endregion

        // AddMaterial
        public void AddMaterial(Material material, Vector2 worldPosition)
        {
            if (material == Material.Gut)
                GutMeter.Add(worldPosition);

            else if (material == Material.Stone)
                StoneMeter.Add(worldPosition);

            else if (material == Material.Wood)
                WoodMeter.Add(worldPosition);

            Session.MaterialsEnabled = true;
        }

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // GutMeter
        public UIMaterialMeter GutMeter { get; }

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

        // StoneMeter
        private UIMaterialMeter StoneMeter { get; }

        // WoodMeter
        private UIMaterialMeter WoodMeter { get; }
    }
}