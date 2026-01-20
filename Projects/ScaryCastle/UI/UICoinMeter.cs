using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UICoinMeter
    /// </summary>
    public class UICoinMeter : GameObject
    {
        private readonly ImageSprite icon;
        private readonly Vector2 iconScale = ScaleInfo.UIElement.Medium;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly UIScore score;

        // Constructor
        public UICoinMeter(EngendroGame game)
            : base(game)
        {
            // Icon
            this.icon = new ImageSprite(Game, Atlases.UI.Coin)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.Area.GetPoint(RectanglePoint.RightBottom, -7, -16),
            };

            // Score
            this.score = new UIScore(game, ColorPalette.Text.Highlight, ScaleInfo.Text.ExtraLarge, false)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            score.Draw(gameTime);
            Game.SpriteBatch.End();
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            icon.Update(gameTime);
            score.Update(gameTime);
        }

        #endregion

        // SetInitialValue
        public void SetInitialValue(int value)
        {
            score.SetInitialValue(value);
        }

        // Value
        public int Value
        {
            get => score.Value;
            set
            {
                if (value != score.Value)
                {
                    score.Value = value;

                    rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
                    icon.Tweens.RotationTween = rotationTween;

                    scaleTween.Start(TweenStyle.QuadraticInOut, iconScale, iconScale * 1.3f, 150, 2);
                    icon.Tweens.ScaleTween = scaleTween;
                }
            }
        }
    }
}
