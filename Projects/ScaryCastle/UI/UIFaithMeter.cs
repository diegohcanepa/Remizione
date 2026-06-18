using Engendro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScaryCastle.UI;

namespace ScaryCastle
{
    /// <summary>
    /// UIFaithMeter
    /// </summary>
    public sealed class UIFaithMeter : GameObject
    {
        #region Private fields

        private readonly UIAmountDisplay amountDisplay;
        private readonly Sprite icon;
        private int lastKnownMaxValue;
        private int lastKnownValue;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();

        #endregion

        #region Constructor

        // Constructor
        public UIFaithMeter()
        {
            float x = 10;
        
            icon = new(Atlases.UI.FaithIcon)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Area.GetPoint(RectanglePoint.LeftBottom, x, -9)
            };

            amountDisplay = new()
            {
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Right, 1, 0),
            };
        }

        #endregion

        #region Private members

        // Animate
        private void Animate()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 15, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.3f, 150, 2);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;
        }

        // Refresh
        private void Refresh()
        {
            if (Run == null)
                return;

            lastKnownValue = Run.CurrentFaith;
            lastKnownMaxValue = Run.FaithThreshold;

            amountDisplay.Current = lastKnownValue;
            amountDisplay.Maximum = lastKnownMaxValue;

            Animate();
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (Run == null)
                return;

            Game.SpriteBatch.Begin(Game.Camera);
            icon.Draw(gameTime);
            Game.SpriteBatch.End();

            amountDisplay.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (Run == null)
                return;

            icon.Update(gameTime);

            if (lastKnownValue != Run.CurrentFaith || lastKnownMaxValue != Run.FaithThreshold)
                Refresh();
        }

        #endregion

        // Run
        public Run? Run
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
