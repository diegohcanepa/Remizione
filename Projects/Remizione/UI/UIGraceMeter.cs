using Adberration;
using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// UIGraceMeter
    /// </summary>
    public class UIGraceMeter : SessionGameObject<GameSession>
    {
        private const int duration = 2000;

        private int deltaScore;
        private bool isInitializing = true;
        private int lastKnownValue;
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
                Color = ColorPalette.Text.TerraDark,
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightBottom, -3, -14),
                Scale = ScaleInfo.Text.Large,
                Text = Localization.GetValue(PlayerStat.Grace)
            };

            // Score text
            this.valueText = new(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Terra,
                PivotOrigin = RectanglePoint.RightTop,
                Scale = ScaleInfo.Text.ExtraLarge,
                Position = titleText.BoundingBox.GetPoint(RectanglePoint.RightBottom)
            };

            isInitializing = true;
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (lastKnownValue == 0 && HideZero)
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
                    valueText.Text = deltaScore.ToString();
                }
            }

            if (Session.PlayerData.Grace != lastKnownValue || isInitializing)
            {
                if (!isInitializing)
                    tween.Start(TweenStyle.Linear, lastKnownValue, Session.PlayerData.Grace, duration);

                lastKnownValue = Session.PlayerData.Grace;
                valueText.Text = lastKnownValue.ToString();
                isInitializing = false;
            }
        }

        #endregion

        // BoundingBox
        public RectangleF BoundingBox => RectangleF.Intersects(valueText.BoundingBox, titleText.BoundingBox);

        // Color
        public Color Color
        {
            get => valueText.Color;
            set => valueText.Color = value;
        }

        // HideZero
        public bool HideZero { get; set; }
    }
}
