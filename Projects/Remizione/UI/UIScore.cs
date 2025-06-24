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
        private readonly ImageSprite icon;
        private bool isInitializing = true;
        private int score;
        private readonly TextSprite scoreText;
        private readonly FloatTween tween = new();

        // Constructor
        public UIScore(RemizioneGame game, AtlasImage iconImage)
            : base(game)
        {
            // Icon
            this.icon = new(game, iconImage)
            {
                Scale = ScaleInfo.UIElement.Small
            };

            // Score text
            this.scoreText = new TextSprite(Game, Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                Scale = ScaleInfo.Text.Huge,
            };

            this.Score = 0;

            isInitializing = true;
        }

        #region Private members

        // Invalidate
        private void Invalidate()
        {
            if (icon.PivotOrigin == RectanglePoint.RightBottom || 
                icon.PivotOrigin == RectanglePoint.RightTop || 
                icon.PivotOrigin == RectanglePoint.Right)
            {
                scoreText.PivotOrigin = RectanglePoint.Right;
                scoreText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, 0, 1);
            }
            else
            {
                scoreText.PivotOrigin = RectanglePoint.Left;
                scoreText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 0, 1);
            }
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (score == 0 && HideZero)
                return;

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.PointClamp);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            Game.SpriteBatch.Begin(Game.Camera, SamplerState.LinearClamp, BlendState.AlphaBlend, null);
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

        // HideZero
        public bool HideZero { get; set; }

        // PivotOrigin
        public RectanglePoint PivotOrigin
        {
            get => icon.PivotOrigin;
            set
            {
                icon.PivotOrigin = value;
                Invalidate();
            }
        }

        // Position
        public Vector2 Position
        {
            get => icon.Position;
            set
            {
                if (value != icon.Position)
                {
                    icon.Position = value;
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
                }
            }
        }
    }
}
