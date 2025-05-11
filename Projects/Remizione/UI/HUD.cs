using Engendro;
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
        private readonly ScoreText gpScore;
        private readonly TextSprite messageText;
        private readonly UIDerivedStats playerStats;
        private readonly ImageSprite savingIcon;
        private readonly UISentence sentence;
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

            // Log
            this.Log = new(Game);

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
                Scale = ScaleInfo.Text.VeryLarge
            };

            // Sentence
            this.sentence = new UISentence(Game);

            // Message text
            this.messageText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.Area.GetPoint(RectanglePoint.Top, 0, 8),
                Scale = ScaleInfo.Text.VeryLarge
            };
        }

        #region Private members

        // FormatSentenceText
        private static string FormatSentenceText(string name)
        {
            const string dots = "...";

            int totalLength = dots.Length + name.Length + dots.Length;

            Span<char> buffer = stackalloc char[totalLength];

            // Copy prefix
            dots.AsSpan().CopyTo(buffer);
            int offset = dots.Length;

            // Copy name
            name.AsSpan().CopyTo(buffer.Slice(offset));
            offset += name.Length;

            // Copy suffix
            dots.AsSpan().CopyTo(buffer.Slice(offset));

            // Return string from buffer
            return new string(buffer);
        }

        // UpdateSentence
        private void UpdateSentence()
        {
            if (session.IsCurrentScene && MouseCursor.Instance.State != MouseCursorState.Wait &&
                session.Player?.InteractiveTarget is GameThing target &&
                (!session.TargetMode || target.CanBeTargeted))
            {
                if (target != sentence.Tag)
                {
                    sentence.Tag = target;
                    sentence.Text = FormatSentenceText(target.LocalizedDisplayName);
                }
            }
            else
            {
                sentence.Text = null;
                sentence.Tag = null;
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            playerStats.Draw(gameTime);

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp);
            messageText.Draw(gameTime);
            Game.SpriteBatch.End();

            if (session.Player != null && session.FullHUD)
                gpScore.Draw(gameTime);

            EchoMessage.Draw(gameTime);

            if (!sentence.IsEmpty && session.IsCurrentScene)
                sentence.Draw(gameTime);

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

            UpdateSentence();
            messageText.Update(gameTime);

            if (session.Player != null)
            {
                gpScore.Score = session.Player.Stats.GP;
                gpScore.Update(gameTime);
            }

            DestinationMark.Update(gameTime);
            EchoMessage.Update(gameTime);
            Log.Update(gameTime);
            savingIcon.Update(gameTime);
        }

        #endregion

        // DestinationMark
        public DestinationMark DestinationMark { get; }

        // EchoMessage
        public EchoMessage EchoMessage { get; }

        // Log
        public UILog Log { get; }

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
