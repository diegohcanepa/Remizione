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
        private readonly FloatTween tween = new();

        // Constructor
        public UIScore(EngendroGame game, Color textColor)
            : base(game)
        {
            // Score text
            this.scoreText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = textColor,
                PivotOrigin = RectanglePoint.Top,
                Scale = ScaleInfo.Text.VeryLarge,
            };

            this.Score = 0;

            isInitializing = true;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            scoreText.Draw(gameTime);
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
        public RectangleF BoundingBox => scoreText.BoundingBox;

        // Color
        public Color Color
        {
            get => scoreText.Color;
            set => scoreText.Color = value;
        }

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
                }
            }
        }
    }
}
