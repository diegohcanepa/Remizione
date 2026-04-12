using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UIHPMeter hpMeter;
        private readonly UIRunProgressMeter progressMeter;
        private readonly TextSprite sacrificeMessage;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
        {
            this.session = session;

            this.hpMeter = new(new(1, 0));
            this.FaithMeter = new();
            this.Log = new();
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Huge);
            this.progressMeter = new(session);

            this.InventoryMeter = new(session.Inventory);

            this.sacrificeMessage = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.LeftBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftBottom, 4, -16),
                Scale = ScaleInfo.Text.Huge,
                Text = TextRepository.GetValue("Misc.SacrificeForFaith")
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            hpMeter.Draw(gameTime);
            FaithMeter.Draw(gameTime);
            progressMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            Message.Draw(gameTime);

            if (IsSacrificeEnabled())
                sacrificeMessage.Draw(gameTime);

            Game.SpriteBatch.End();

            if (session.IsCurrentScene)
            {
                if (session.Room is RideRoom rideRoom && rideRoom.RoomNode.RoomType == RoomType.Corridor)
                    GuardMeter.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            GuardMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            FaithMeter.Update(gameTime);
            progressMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
        }

        #endregion

        // FaithMeter
        public UIFaithMeter FaithMeter;

        // GuardMeter
        public UIGuardMeter GuardMeter { get; } = new();

        // HandleInput
        public HandleInputResult HandleInput()
        {
            if (session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            return HandleInputResult.Unhandled;
        }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // IsSacrificeEnabled
        public bool IsSacrificeEnabled()
        {
            return session.InteractionContext.HeldItem != null && FaithMeter.BoundingBox.Contains(InputManager.DefaultPlayer.Mouse.VirtualPosition);
        }

        // Log
        public UILog Log { get; }

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            FaithMeter.Actor = session.Player;
        }
    }
}
