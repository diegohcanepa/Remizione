using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Remizione.UI;

namespace Remizione
{
    /// <summary>
    /// HUD
    /// </summary>
    public sealed class HUD : GameObject
    {
        private readonly UIDerivedStats playerStats;
        private readonly ScoreText gpScore;
        private readonly TextSprite messageText;
        private readonly TextSprite narrationText;
        private readonly ImageSprite savingIcon;
        private readonly TextSprite sentenceText;
        private readonly GameSession session;

        // Constructor
        public HUD(GameSession session)
            : base(session.Game)
        {
            this.session = session;

            this.playerStats = new(session.Game);

            // DestinationMark
            this.DestinationMark = new DestinationMark(session);

            // Echo message
            this.EchoMessage = new EchoMessage(session.Game);

            // Saving icon
            this.savingIcon = new ImageSprite(Game, Atlases.UI.SavingIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -8, 6)
            };

            // GP score
            this.gpScore = new ScoreText(session.Game)
            {
                HideZero = true,
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.RightBottom, -2, 0),
                Scale = ScaleInfo.Text.Large
            };

            // Prompt
            this.sentenceText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.SafeArea.GetPoint(RectanglePoint.Bottom, 0, -5),
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Message text
            this.messageText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.TextDepracated.Dark,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 8),
                Scale = ScaleInfo.Text.Large
            };

            // Narration text
            this.narrationText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                MaximumWidth = (int)(Screen.NativeWidth * .7f),
                PauseOnPunctuationMarks = false,
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.Area.GetPoint(RectanglePoint.Bottom, 0, -10),
                Scale = ScaleInfo.Text.Large
            };
        }

        #region Private members

        // UpdatePrompt
        private void UpdatePrompt()
        {
            if (session.IsCurrentScene && MouseCursor.Instance.State != MouseCursorState.Wait &&
                session.Player?.InteractiveTarget is GameThing target &&
                (!session.TargetMode || target.CanBeTargeted))
            {
                if (target != sentenceText.Tag)
                {
                    sentenceText.Tag = target;
                    sentenceText.Text = "..." + target.GetLocalizedDisplayName() + "...";
                }
            }
            else
            {
                sentenceText.Text = null;
                sentenceText.Tag = null;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            playerStats.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            if (narrationText.IsEmpty)
                sentenceText.Draw(gameTime);
            else
                narrationText.Draw(gameTime);

            messageText.Draw(gameTime);

            Game.SpriteBatch.End();

            if (session.Player != null && session.FullHUD)
            {
                gpScore.Draw(gameTime);
            }

            EchoMessage.Draw(gameTime);

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

            UpdatePrompt();
            narrationText.Update(gameTime);
            messageText.Update(gameTime);

            if (session.Player != null)
            {
                gpScore.Score = session.Player.Stats.GP;
                gpScore.Update(gameTime);
            }

            DestinationMark.Update(gameTime);
            EchoMessage.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // EchoMessage
        public EchoMessage EchoMessage { get; }

        // MessageText
        public string MessageText
        {
            get => messageText.Text ?? string.Empty;
            set
            {
                if (messageText.Text != value)
                {
                    messageText.Text = value;
                    messageText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 500);
                }
            }
        }

        // NarrationText
        public string NarrationText
        {
            get => narrationText.Text ?? string.Empty;
            set
            {
                if (narrationText.Text != value)
                {
                    narrationText.Text = value;
                    narrationText.Tweens.OpacityTween = FloatTween.Create(TweenStyle.CubicInOut, 0, 1, 500);
                }
            }
        }

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
    }
}
