using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Remizione.UI
{
    /// <summary>
    /// UIScore
    /// </summary>
    public class UIScore : GameObject
    {
        private const int duration = 2000;

        private int deltaScore;
        private bool isInitializing = true;
        private int score;
        private readonly TextSprite scoreText;
        private readonly TextSprite titleText;
        private readonly FloatTween tween = new();

        // Constructor
        public UIScore(RemizioneGame game, string title)
            : base(game)
        {
            // Score text
            this.scoreText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                Scale = ScaleInfo.Text.VeryLarge,
            };

            // Title text
            this.titleText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.TerraDark,
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.Text.Large,
                Text = title
            };

            this.Score = 0;

            isInitializing = true;
        }

        #region Invalidate

        // Invalidate
        private void Invalidate()
        {
            scoreText.Position = titleText.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1);
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (score == 0 && HideZero)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, BlendState.AlphaBlend, null);
            scoreText.Draw(gameTime);
            titleText.Draw(gameTime);
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
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => RectangleF.Intersects(scoreText.BoundingBox, titleText.BoundingBox);

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
            set
            {
                scoreText.PivotOrigin = value;
                titleText.PivotOrigin = value;
            }
        }

        // Position
        public Vector2 Position
        {
            get => titleText.Position;
            set
            {
                if (value != titleText.Position)
                {
                    titleText.Position = value;
                    Invalidate();
                }
            }
        }

        // Score
        public int Score
        {
            get => score;
            set
            {
                if (value != score || isInitializing)
                {
                    if (!isInitializing)
                        tween.Start(TweenStyle.Linear, score, value, duration);

                    score = value;
                    scoreText.Text = score.ToString();
                    isInitializing = false;
                    Invalidate();
                }
            }
        }
    }
}
