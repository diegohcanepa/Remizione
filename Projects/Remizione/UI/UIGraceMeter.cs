using Adberration;
using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Remizione
{
    /// <summary>
    /// UIGraceMeter
    /// </summary>
    public class UIGraceMeter : SessionGameObject<GameSession>
    {
        private const int duration = 2000;

        private readonly ColorTween colorTween = new();
        private int deltaScore;
        private bool isInitializing = true;
        private int lastKnownValue;
        private readonly Color titleColor = ColorPalette.Text.TerraDark;
        private readonly TextSprite titleText;
        private readonly FloatTween tween = new();
        private readonly TextSprite valueText;

        // Constructor
        public UIGraceMeter(GameSession session)
            : base(session)
        {
            // Title text
            this.titleText = new(Fonts.CommonOutline)
            {
                Color = titleColor,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -3, -12),
                Scale = ScaleInfo.Text.Medium,
                Text = Localization.GetValue(PlayerStat.Grace)
            };

            // Value text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.Text.Large,
                Position = titleText.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1)
            };

            isInitializing = true;
        }

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

            if (Session.PlayerData.Grace != lastKnownValue || isInitializing)
            {
                if (!isInitializing)
                {
                    colorTween.Start(TweenStyle.CubicInOut, titleColor, ColorPalette.MouseCursor.Tooltip * .7f, 1000, 2);
                    titleText.Tweens.ColorTween = colorTween;
                    tween.Start(TweenStyle.Linear, lastKnownValue, Session.PlayerData.Grace, duration);
                }

                lastKnownValue = Session.PlayerData.Grace;
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
