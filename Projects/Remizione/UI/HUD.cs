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

        private readonly UICycleMeter cycleMeter;
        private readonly UIScore grace;
        private readonly UIDerivedStats playerStats;
        private readonly UIPrompt prompt;
        private readonly ImageSprite savingIcon;
        private readonly GameSession session;
        private readonly TextSprite statusText;

        #endregion

        #region Constructor

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.playerStats = new(session.Game);
            this.cycleMeter = new(session);

            // CraftPositionMark
            this.CraftingMark = new CraftingMark(session);

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

            // Grace
            this.grace = new UIScore(session.Game, Atlases.UI.GraceIcon)
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

                playerStats.Draw(gameTime);

                grace.Draw(gameTime);

                if (!savingIcon.Tweens.IsTweening)
                    cycleMeter.Draw(gameTime);
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
            CraftingMark.Update(gameTime);
            QuickSlot.Update(gameTime);
            playerStats.Update(gameTime);
            prompt.Update(gameTime);

            if (session.PurgatoryMode)
            {
                cycleMeter.Update(gameTime);
                statusText.Update(gameTime);
            }

            if (session.Player != null)
            {
                grace.Score = session.Player.Grace;
                grace.Update(gameTime);
            }

            Log.Update(gameTime);
            Message.Update(gameTime);

            savingIcon.Update(gameTime);
        }

        #endregion

        // CraftingMark
        public CraftingMark CraftingMark { get; }

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
            playerStats.Actor = session.Player;
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
