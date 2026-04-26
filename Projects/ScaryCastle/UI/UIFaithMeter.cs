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

        private readonly Sprite[] icons = new Sprite[5];
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        // Constructor
        public UIFaithMeter()
        {
            float x = 12;
            for (var i = 0; i < icons.Length; i++)
            {
                icons[i] = new(Atlases.UI.FaithIcons[0])
                {
                    PivotOrigin = RectanglePoint.Center,
                    Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, x, -12)
                };

                x += icons[i].BoundingBox.Width;
            }
        }

        #region Private members

        // Refresh
        private void Refresh()
        {
            if (Actor == null)
                return;

            for (var i = 0; i < Actor.MaxFaith; i++)
            {
                icons[i].RenderImage = Atlases.UI.FaithIcons[0];
            }

            for (var i = 0; i < Actor.Faith; i++)
            {
                icons[i].RenderImage = Atlases.UI.FaithIcons[1];
            }

            lastKnownValue = Actor.Faith;
            lastKnownMaxValue = Actor.MaxFaith;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < Actor.MaxFaith; i++)
            {
                icons[i].Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Actor == null)
                return;

            for (var i = 0; i < Actor.MaxFaith; i++)
            {
                icons[i].Update(gameTime);
            }

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
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 4);

            //icon.Tweens.RotationTween = rotationTween;
            //icon.Tweens.ScaleTween = scaleTween;
        }
    }
}
