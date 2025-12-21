using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScaryCastle
{
    /// <summary>
    /// UITicketMeter
    /// </summary>
    public class UITicketMeter : GameObject
    {
        private readonly ImageSprite icon;
        private readonly Vector2 iconScale = ScaleInfo.UIElement.Tiny;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly UIScore score;
        private readonly ImageSprite slot;

        // Constructor
        public UITicketMeter(EngendroGame game)
            : base(game)
        {
            // Slot
            this.slot = new ImageSprite(Game, Atlases.UI.TicketSlot)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.LeftTop, 205, 110)
            };

            // Icon
            this.icon = new ImageSprite(Game, Atlases.UI.TicketIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = slot.BoundingBox.GetPoint(RectanglePoint.Center, 0, -.5f),
                Scale = iconScale
            };

            // Score
            this.score = new UIScore(game, ColorPalette.Text.Default, ScaleInfo.Text.Large, false)
            {
                PivotOrigin = RectanglePoint.Top,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Bottom, 0, 1)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(Game.Camera);
            slot.Draw(gameTime);
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
