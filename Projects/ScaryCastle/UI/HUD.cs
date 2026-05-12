using Engendro;
using Engendro.Audio;
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

        private readonly TextSprite attackName;
        private readonly UIHPMeter hpMeter;
        private readonly GameSession session;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
        {
            this.session = session;

            this.attackName = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Orange,
                PivotOrigin = RectanglePoint.LeftTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 2, 7),
                Scale = ScaleInfo.Text.Huge
            };

            this.hpMeter = new(new(4, 2));
            this.Message = new(RectanglePoint.Top, Screen.HUDArea.GetPoint(RectanglePoint.Top, 0, 5), ScaleInfo.Text.Huge);
            this.Countdown = new(session);
            this.InventoryMeter = new(session.PlayerInventory);
            this.PassiveItems = new(session);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);

            if (session.InteractionContext.Mode == InteractionContextMode.Attack)
                attackName.Draw(gameTime);

            hpMeter.Draw(gameTime);
            FaithMeter.Draw(gameTime);
            InventoryMeter.Draw(gameTime);
            Log.Draw(gameTime);
            Message.Draw(gameTime);
            Countdown.Draw(gameTime);
            PassiveItems.Draw(gameTime);

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
            PassiveItems.Update(gameTime);
            InventoryMeter.Update(gameTime);
            GuardMeter.Update(gameTime);
            hpMeter.Update(gameTime);
            FaithMeter.Update(gameTime);
            Log.Update(gameTime);
            Message.Update(gameTime);
            Countdown.Update(gameTime);

            if (session.Player != null)
            {
                if (attackName.Tag != session.Player.CombatBehavior?.DefaultIntent)
                    attackName.Text = session.Player.CombatBehavior?.DefaultIntent?.DisplayName;
            }
        }

        #endregion

        // AttackName
        public string? AttackName
        {
            get => attackName.Text;
            set => attackName.Text = value;
        }

        // FaithMeter
        public UIFaithMeter FaithMeter { get; } = new();

        // GateMeter
        public UICountdown Countdown { get; }

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

        // Log
        public UILog Log { get; } = new();

        // Message
        public HUDMessage Message { get; }

        // PassiveItems
        public UIPassiveItems PassiveItems { get; }

        // Reset
        public void Reset()
        {
            hpMeter.Actor = session.Player;
            FaithMeter.Actor = session.Player;
            Message.Hide();
            Log.Hide();
            GuardMeter.Target = null;
        }
    }
}
