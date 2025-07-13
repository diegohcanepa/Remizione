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

        private readonly UIHealthMeter healthMeter;
        private readonly UIPrompt prompt;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly TextSprite statusText;
        private readonly UIScore tickets;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.healthMeter = new(session.Game);

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

            // Tickets
            this.tickets = new UIScore(session.Game, Atlases.UI.TicketsIcon, ColorPalette.Text.Default)
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -4, -4),
            };

            // Prompt
            this.prompt = new(session);

            // Quick slot
            this.QuickSlot = new(Game);

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
            if (session.PurgatoryMode)
            {
                if (!session.IsConsoleVisible)
                    QuickSlot.Draw(gameTime);

                healthMeter.Draw(gameTime);

                //playerStats.Draw(gameTime);

                tickets.Draw(gameTime);

                //if (!savingIcon.Tweens.IsTweening)
                //    cycleMeter.Draw(gameTime);
            }

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
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
            QuickSlot.Update(gameTime);
            healthMeter.Update(gameTime);
            prompt.Update(gameTime);

            if (session.PurgatoryMode)
                statusText.Update(gameTime);

            if (session.Player != null)
            {
                tickets.Score = session.Player.Tickets;
                tickets.Update(gameTime);
            }

            Log.Update(gameTime);
            Message.Update(gameTime);

            savingIcon.Update(gameTime);
        }

        #endregion

        // HandleInput
        public HandleInputResult HandleInput(GameTime gameTime) => QuickSlot.HandleInput(gameTime);

        // Log
        public UILog Log { get; }

        // Message
        public HUDMessage Message { get; }

        // QuickSlot
        public QuickSlot QuickSlot { get; }

        // Reset
        public void Reset()
        {
            healthMeter.Actor = session.Player;
            QuickSlot.Actor = session.Player;
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
    }
}
