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

        private readonly TextSprite amountText;
        private readonly ColorTween colorTween = new();
        private int deltaScore;
        private readonly Sprite icon;
        private bool isInitializing = true;
        private int lastKnownValue;
        private readonly Color titleColor = Colors.Palette.SteelBlue;
        private readonly TextSprite titleText;
        private readonly FloatTween tween = new();

        #region Constructor

        // Constructor
        public UIGraceMeter(GameSession session)
            : base(session)
        {
            // Title text
            this.titleText = new(Fonts.CommonOutline)
            {
                Color = titleColor,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -3, -15),
                Scale = ScaleInfo.Text.Medium,
                Text = Localization.GetValue(PlayerStat.Grace)
            };

            // Icon
            this.icon = new(Atlases.UI.GraceIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = titleText.BoundingBox.GetPoint(RectanglePoint.RightBottom, 0, -1),
            };

            // Amount text
            this.amountText = new(Fonts.CommonOutline)
            {
                Color = Colors.Palette.SteelBlue,
                PivotOrigin = RectanglePoint.Right,
                Scale = ScaleInfo.Text.VeryLarge,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, -1, 1)
            };

            isInitializing = true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            //if (lastKnownValue == 0)
              //  return;

            amountText.Draw(gameTime);

            if (tween.IsRunning)
                titleText.Draw(gameTime);

            icon.Draw(gameTime);
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
                    amountText.Text = deltaScore.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (Session.PlayerData.Grace != lastKnownValue || isInitializing)
            {
                if (!isInitializing)
                {
                    if (Session.PlayerData.Grace > lastKnownValue)
                    {
                        colorTween.Start(TweenStyle.CubicInOut, titleColor, Colors.Palette.SkyBlue, 1000, 2);
                        titleText.Tweens.ColorTween = colorTween;
                    }

                    tween.Start(TweenStyle.Linear, lastKnownValue, Session.PlayerData.Grace, duration);
                }

                lastKnownValue = Session.PlayerData.Grace;
                amountText.Text = lastKnownValue.ToString(CultureInfo.InvariantCulture);
                isInitializing = false;
            }

            titleText.Update(gameTime);

            amountText.Color = titleText.Color;
        }

        #endregion

        // Color
        public Color Color
        {
            get => amountText.Color;
            set => amountText.Color = value;
        }
    }
}
