using Adberration;
using Engendro;
using Engendro.Audio;
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

        private readonly UIStatuses statuses;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session)
        {
            this.CycleMeter = new(session);
            this.PlayerHPMeter = new(session);
            this.InventoryMeter = new(session.PlayerData.Inventory);
            this.DestinationMark = new(session);
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 10));
            this.BottomMessage = new(RectanglePoint.Bottom, Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -9));
            this.AshMeter = new(session);
            this.statuses = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            if (Session.DisplayHPMeter)
            {
                PlayerHPMeter.Draw(gameTime);
                CycleMeter.Draw(gameTime);
            }

            AshMeter.Draw(gameTime);
            statuses.Draw(gameTime);
            Game.SpriteBatch.End();

            InventoryMeter.Draw(gameTime);

            Message.Draw(gameTime);
            BottomMessage.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            InventoryMeter.Update(gameTime);
            PlayerHPMeter.Update(gameTime);
            Message.Update(gameTime);
            BottomMessage.Update(gameTime);
            AshMeter.Update(gameTime);
            statuses.Update(gameTime);
            DestinationMark.Update(gameTime);
            CycleMeter.Update(gameTime);

            if (InventoryMeter.IsMouseOver())
                HoveredComponent = InventoryMeter;

            else if (AshMeter.IsMouseOver())
                HoveredComponent = AshMeter;

            else if (CycleMeter.IsMouseOver())
                HoveredComponent = CycleMeter;

            else if (PlayerHPMeter.IsMouseOver())
                HoveredComponent = PlayerHPMeter;

            else
                HoveredComponent = null;
        }

        #endregion

        // AshMeter
        public UIAshMeter AshMeter { get; }

        // BottomMessage
        public UIMessage BottomMessage { get; }

        // CycleMeter
        public UICycleMeter CycleMeter { get; }

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (Session.IsAwaiting)
                return HandleInputResult.Unhandled;

            if (Session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            if (HoveredComponent != null && InputManager.DefaultPlayer.Mouse.IsLeftButtonPressed())
            {
                Sound.Play(SoundNames.Bones);
                return HandleInputResult.Handled;
            }

            return HandleInputResult.Unhandled;
        }

        // HoveredComponent
        public HUDComponent? HoveredComponent { get; private set; }

        // InventoryMeter
        public UIInventoryMeter InventoryMeter { get; }

        // Message
        public UIMessage Message { get; }

        // PlayerHPMeter
        public UIPlayerHPMeter PlayerHPMeter { get; }

        // Reset
        public void Reset()
        {
            Message.Hide();
            statuses.Actor = Session.Player;
        }
    }
}