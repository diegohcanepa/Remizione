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
        public UIScore(EngendroGame game, AtlasImage iconImage, Color textColor)
            : base(game)
        {
            // Icon
            this.icon = new(game, iconImage)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.UIElement.Tiny
            };

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

        // Position
        public Vector2 Position
        {
            get => icon.Position;
            set
            {
                if (value != icon.Position)
                {
                    icon.Position = value;
                    scoreText.Position = icon.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, -3);
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
