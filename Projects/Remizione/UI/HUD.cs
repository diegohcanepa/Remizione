using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject, IInputHandler
    {
        #region Private fields

        private readonly UICountdownMeter countdownMeter;
        private readonly UIHealthMeter healthMeter;
        private readonly UIPrompt prompt;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly TextSprite statusText;
        private readonly UIProgressMeter progressMeter;
        private readonly UITicketsMeter ticketsMeter;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.countdownMeter = new(session);
            this.healthMeter = new(session.Game);
            this.progressMeter = new(session);
            this.ticketsMeter = new(session.Game);

            // Log
            this.Log = new(Game);

            // Message
            this.Message = new(Game);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -6, 3),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Prompt
            this.prompt = new(session);

            // Bag slot
            this.BagSlot = new(Game);

            // Equipment slot
            this.EquipmentSlot = new(Game);

            // Trincket slot
            this.TrincketSlot = new(Game);

            // Message text
            this.statusText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 8),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (session.GameplayMode == GameplayMode.Survival)
            {
                if (!session.IsConsoleVisible)
                {
                    BagSlot.Draw(gameTime);
                    EquipmentSlot.Draw(gameTime);
                    TrincketSlot.Draw(gameTime);
                }

                healthMeter.Draw(gameTime);

                if (session.IsCountdownActive)
                    countdownMeter.Draw(gameTime);
                else
                    progressMeter.Draw(gameTime);

                ticketsMeter.Draw(gameTime);
            }

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            statusText.Draw(gameTime);
            Game.SpriteBatch.End();

            prompt.Draw(gameTime);

            Log.Draw(gameTime);
            Message.Draw(gameTime);

            if (savingIcon.Tweens.IsTweening)
            {
                Game.SpriteBatch.Begin(Game.Camera);
                savingIcon.Draw(gameTime);
                Game.SpriteBatch.End();
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            countdownMeter.Update(gameTime);
            BagSlot.Update(gameTime);
            EquipmentSlot.Update(gameTime);
            TrincketSlot.Update(gameTime);
            healthMeter.Update(gameTime);
            progressMeter.Update(gameTime);
            ticketsMeter.Update(gameTime);

            prompt.Update(gameTime);

            if (session.GameplayMode == GameplayMode.Survival)
                statusText.Update(gameTime);

            Log.Update(gameTime);
            Message.Update(gameTime);

            savingIcon.Update(gameTime);
        }

        #endregion

        // BagSlot
        public BagSlot BagSlot { get; }

        // EquipmentSlot
        public EquipmentSlot EquipmentSlot { get; }

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime)
        {
            if (session.IsConsoleVisible)
                return HandleInputResult.Unhandled;

            if (EquipmentSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            if (BagSlot.HandleInput(gameTime) == HandleInputResult.Handled)
                return HandleInputResult.Handled;

            return HandleInputResult.Unhandled;
        }

        // Log
        public UILog Log { get; }

        // Message
        public HUDMessage Message { get; }

        // Reset
        public void Reset()
        {
            healthMeter.Actor = session.Player;
            BagSlot.Actor = session.Player;
            EquipmentSlot.Actor = session.Player;
            TrincketSlot.Actor = session.Player;
            ticketsMeter.Actor = session.Player;
        }

        // ShowSavingIcon
        public void ShowSavingIcon()
        {
            savingIcon.Tweens.OpacityTween = FloatTween.Create(TweenStyle.QuadraticInOut, 1, .8f, 300, 10);
        }

        // Status
        public string Status
        {
            get => statusText.Text ?? string.Empty;
            set
            {
                if (statusText.Text != value)
                {
                    statusText.Text = value;
                    statusText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 500);
                }
            }
        }

        // TrincketSlot
        public TrinketSlot TrincketSlot { get; }
    }
}
