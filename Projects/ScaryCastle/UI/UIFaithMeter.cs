using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIFaithMeter
    /// </summary>
    public sealed class UIFaithMeter : GameObject
    {
        #region Private fields

        private readonly TextSprite amountText;
        private readonly Sprite icon;
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly Vector2Tween scaleTween = new();

        #endregion

        // Constructor
        public UIFaithMeter()
        {
            // Icon
            this.icon = new(Atlases.UI.FaithIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, 12, -12),
            };

            // Amount
            this.amountText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Highlight,
                PivotOrigin = RectanglePoint.Left,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 1, 1),
                Scale = ScaleInfo.Text.Huge,
                Spacing = -6
            };
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            lastKnownValue = Actor.Faith;
            lastKnownMaxValue = Actor.MaxFaith;

            amountText.Text = $"{lastKnownValue}/{lastKnownMaxValue}";
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            icon.Update(gameTime);

            if (Actor == null)
                return;

            icon.Draw(gameTime);
            amountText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            if (lastKnownValue != Actor.Faith || lastKnownMaxValue != Actor.MaxFaith)
                Refresh();
        }

        #endregion

        // Actor
        public Actor? Actor
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;

                    if (field == null)
                    {
                        lastKnownValue = int.MinValue;
                        lastKnownMaxValue = int.MinValue;
                    }

                    Refresh();
                }
            }
        }

        // Animate
        public void Animate()
        {
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 2);

            icon.Tweens.ScaleTween = scaleTween;
        }
    }
}
