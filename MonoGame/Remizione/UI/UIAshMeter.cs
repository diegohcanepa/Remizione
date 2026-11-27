using Engendro;
using Microsoft.Xna.Framework;
using Remizione.UI;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIAshMeter
    /// </summary>
    public class UIAshMeter : HUDComponent
    {
        private const int duration = 2000;

        private int deltaScore;
        private bool isInitializing = true;
        private int lastKnownValue;
        private readonly Sprite icon;
        private readonly FloatTween rotationTween = new();
        private readonly Vector2Tween scaleTween = new();
        private readonly FloatTween tween = new();
        private readonly TextSprite valueText;

        #region Constructor

        // Constructor
        public UIAshMeter(GameSession session)
            : base(session)
        {
            // Icon
            this.icon = new(Atlases.UI.CineraryUrn)
            {
                PivotOrigin = RectanglePoint.RightBottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -3, 0),
            };

            // Value text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Terra,
                PivotOrigin = RectanglePoint.RightBottom,
                Scale = ScaleInfo.Text.VeryLarge,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -1, 1)
            };

            isInitializing = true;

            BoundingBox = icon.BoundingBox;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!Session.AshesEnabled)
                return;

            icon.Draw(gameTime);
            valueText.Draw(gameTime);
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
                    valueText.Text = deltaScore.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (Session.PlayerData.Ashes != lastKnownValue || isInitializing)
            {
                if (!isInitializing)
                {
                    Animate();
                    tween.Start(TweenStyle.Linear, lastKnownValue, Session.PlayerData.Ashes, duration);
                }

                lastKnownValue = Session.PlayerData.Ashes;
                valueText.Text = lastKnownValue == 0 ? string.Empty : lastKnownValue.ToString(CultureInfo.InvariantCulture);
                isInitializing = false;
            }

            icon.Update(gameTime);
        }

        #endregion

        // Animate
        public void Animate()
        {
            rotationTween.Start(TweenStyle.QuadraticInOut, 0, 10, 50, 6);
            scaleTween.Start(TweenStyle.QuadraticInOut, Vector2.One, Vector2.One * 1.1f, 150, 2);

            icon.Tweens.RotationTween = rotationTween;
            icon.Tweens.ScaleTween = scaleTween;

            //Sound.Play(SoundNames.ItemAdd);
        }
    }
}
