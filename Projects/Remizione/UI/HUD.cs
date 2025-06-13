using Engendro;
using Engendro.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;
using System;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject
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

            // DestinationMark
            this.DestinationMark = new DestinationMark(session);

            // Log
            this.Log = new(Game);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -6, 3),
                Scale = ScaleInfo.UIElement.Medium
            };

            // Grace
            this.grace = new UIScore(session.Game, TextRepository.GetValue("ActorProperty.Grace.Name"))
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -5, -15),
            };

            // Prompt
            this.prompt = new(session);

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
            if (session.FullHUD)
            {
                playerStats.Draw(gameTime);

                if (!savingIcon.Tweens.IsTweening)
                    cycleMeter.Draw(gameTime);
            }

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            statusText.Draw(gameTime);
            Game.SpriteBatch.End();

            prompt.Draw(gameTime);

            if (session.Player != null && session.FullHUD)
                //grace.Draw(gameTime);

            Log.Draw(gameTime);

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
            playerStats.Update(gameTime);
            prompt.Update(gameTime);
            cycleMeter.Update(gameTime);
            statusText.Update(gameTime);

            if (session.Player != null)
            {
                grace.Score = session.Player.Grace;
                grace.Update(gameTime);
            }

            DestinationMark.Update(gameTime);
            Log.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // Log
        public UILog Log { get; }

        // Reset
        public void Reset()
        {
            playerStats.Actor = session.Player;
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
