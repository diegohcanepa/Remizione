using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// ScoreText
    /// </summary>
    public sealed class ScoreText : GameObject
    {
        private const int duration = 2000;

        private int deltaScore;
        private readonly FloatTween deltaScoreOpacityTween = new() { StartDelay = duration - 300 };
        private readonly TextSprite deltaScoreText;
        private bool isInitializing = true;
        private int score;
        private readonly TextSprite scoreText;
        private readonly FloatTween tween = new();

        // Constructor
        public ScoreText(RemizioneGame game)
            : base(game)
        {
            this.deltaScoreText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default,
                PivotOrigin = RectanglePoint.RightBottom
            };

            this.scoreText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Default
            };

            this.Scale = ScaleInfo.Text.Medium;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (score == 0 && HideZero)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, BlendState.AlphaBlend, null);
            deltaScoreText.Draw(gameTime);
            scoreText.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                int newDeltaScore = (int)tween.CurrentValue;
                if (newDeltaScore != deltaScore)
                {
                    deltaScore = newDeltaScore;
                    scoreText.Text = deltaScore.ToString();
                }
            }

            deltaScoreText.Update(gameTime);
        }

        #endregion

        // Color
        public Color Color
        {
            get => scoreText.Color;
            set => scoreText.Color = value;
        }

        // HideZero
        public bool HideZero { get; set; }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => scoreText.PivotOrigin;
            set => scoreText.PivotOrigin = value;
        }

        // Position
        public Vector2 Position
        {
            get => scoreText.Position;
            set => scoreText.Position = value;
        }

        // Scale
        public Vector2 Scale
        {
            get => scoreText.Scale;
            set
            {
                scoreText.Scale = value;
                deltaScoreText.Scale = value * .8f;
            }
        }

        // Score
        public int Score
        {
            get => score;
            set
            {
                if (value != score)
                {
                    if (!isInitializing)
                    {
                        deltaScoreText.Text = $"+{value - score}";
                        deltaScoreText.Position = scoreText.BoundingBox.GetPoint(RectanglePoint.RightTop, 0, 1);
                        deltaScoreOpacityTween.Start(TweenStyle.CubicOut, 1, 0, 300);
                        deltaScoreText.Tweens.OpacityTween = deltaScoreOpacityTween;
                        tween.Start(TweenStyle.Linear, score, value, duration);
                    }

                    score = value;
                    scoreText.Text = score.ToString();
                    isInitializing = false;
                }
            }
        }
    }
}
