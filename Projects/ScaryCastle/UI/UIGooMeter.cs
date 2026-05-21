using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// UIGooMeter
    /// </summary>
    public sealed class UIGooMeter : GameObject
    {
        private const int MaxGoo = 5;

        #region Private fields

        private readonly Sprite[] icons = new Sprite[MaxGoo];
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly FloatTween[] rotationTween = new FloatTween[MaxGoo];
        private readonly Vector2Tween[] scaleTween = new Vector2Tween[MaxGoo];

        #endregion

        #region Constructor

        // Constructor
        public UIGooMeter()
        {
            float x = 10;
            for (var i = 0; i < MaxGoo; i++)
            {
                icons[i] = new(Atlases.UI.GooIcons[0])
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, x, -8)
                };

                x += icons[i].BoundingBox.Width;

                rotationTween[i] = new FloatTween();
                scaleTween[i] = new Vector2Tween();
            }
        }

        #endregion

        #region Private members

        // Animate
        private void Animate(int index, int delay)
        {
            rotationTween[index].StartDelay = delay;
            scaleTween[index].StartDelay = delay;

            rotationTween[index].Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween[index].Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 4);

            icons[index].Tweens.RotationTween = rotationTween[index];
            icons[index].Tweens.ScaleTween = scaleTween[index];
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

            var delay = 0;
            var animationCount = Actor.Goo - lastKnownValue;
            for (var i = Actor.Goo - 1; i >= 0; i--)
            {
                if (animationCount > 0)
                {
                    Animate(i, delay);
                    delay += 500;
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

                    for (var i = 0; i < MaxGoo; i++)
                    {
                        icons[i].Tweens.Reset();
                    }

                    Refresh();
                }
            }
        }
    }
}
