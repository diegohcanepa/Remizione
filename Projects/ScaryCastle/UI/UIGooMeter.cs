using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIGooMeter
    /// </summary>
    public sealed class UIGooMeter : GameObject
    {
        #region Private fields

        private readonly Sprite[] icons = new Sprite[5];
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        // Constructor
        public UIGooMeter()
        {
            float x = 10;
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(Atlases.UI.GooIcons[0])
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, x, -8)
                };

                x += icons[i].BoundingBox.Width;
            }
        }

        #region Private members

        // Animate
        private void Animate(Sprite icon)
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 4);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;
        }

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            for (var i = 0; i < Actor.MaxGoo; i++)
            {
                icons[i].RenderImage = Atlases.UI.GooIcons[0];
            }

            var animationCount = Actor.Goo - lastKnownValue;
            for (var i = Actor.Goo - 1; i >= 0; i--)
            {
                if (animationCount > 0)
                {
                    Animate(icons[i]);
                    animationCount--;
                }

                icons[i].RenderImage = Atlases.UI.GooIcons[1];
            }

            lastKnownValue = Actor.Goo;
            lastKnownMaxValue = Actor.MaxGoo;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < Actor.MaxGoo; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < Actor.MaxGoo; i++)
            {
                icons[i].Update(gameTime);
            }

            if (lastKnownValue != Actor.Goo || lastKnownMaxValue != Actor.MaxGoo)
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
    }
}
