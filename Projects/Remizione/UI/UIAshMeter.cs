using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIAshMeter
    /// </summary>
    public class UIAshMeter : SessionGameObject<GameSession>
    {
        private const int duration = 2000;

        private readonly ColorTween colorTween = new();
        private int deltaScore;
        private bool isInitializing = true;
        private int lastKnownValue;
        private readonly Color titleColor = Colors.Palette.TerraDark;
        private readonly TextSprite titleText;
        private readonly FloatTween tween = new();
        private readonly TextSprite valueText;

        #region Constructor

        // Constructor
        public UIAshMeter(GameSession session)
            : base(session)
        {
            // Title text
            this.titleText = new(Fonts.CommonOutline)
            {
                Color = titleColor,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -3, -13),
                Scale = ScaleInfo.Text.Large,
                Text = Localization.GetValue(PlayerStat.Ashes)
            };

            // Value text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = Colors.Palette.Terra,
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.Text.VeryLarge,
                Position = titleText.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1)
            };

            isInitializing = true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (lastKnownValue == 0)
                return;

            valueText.Draw(gameTime);
            titleText.Draw(gameTime);
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
                    colorTween.Start(TweenStyle.CubicInOut, titleColor, Colors.Palette.Terra, 1000, 2);
                    titleText.Tweens.ColorTween = colorTween;
                    tween.Start(TweenStyle.Linear, lastKnownValue, Session.PlayerData.Ashes, duration);
                }

                lastKnownValue = Session.PlayerData.Ashes;
                valueText.Text = lastKnownValue.ToString(CultureInfo.InvariantCulture);
                isInitializing = false;
            }

            titleText.Update(gameTime);
        }

        #endregion

        // Color
        public Color Color
        {
            get => valueText.Color;
            set => valueText.Color = value;
        }
    }
}
