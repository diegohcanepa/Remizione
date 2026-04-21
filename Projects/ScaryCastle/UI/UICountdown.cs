using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// UICountdown
    /// </summary>
    public sealed class UICountdown : GameObject
    {
        private readonly Sprite icon;
        private double lastKnownValue;
        private GameSession session;
        private float timeLeft;
        private readonly TextSprite timeText;

        // Constructor
        public UICountdown(GameSession session)
        {
            this.session = session;

            // Icon
            this.icon = new(Atlases.UI.CorridorDoorIcon)
            {
                PivotOrigin = RectanglePoint.RightTop,
                Position = Screen.Area.GetPoint(RectanglePoint.RightTop, -4, 3),
            };

            // Amount
            this.timeText = new TextSprite(Fonts.CommonOutline)
            {
                Color = ColorPalette.Text.Yellow,
                PivotOrigin = RectanglePoint.Right,
                Position = icon.BoundingBox.GetPoint(RectanglePoint.Left, 0, 1),
                Scale = ScaleInfo.Text.ExtraGiant,
                Spacing = -6
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsRunning)
                return;

            icon.Draw(gameTime);
            timeText.Draw(gameTime);
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (!IsRunning || timeLeft <= 0)
                return;

            timeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                IsRunning = false;
                session.AwaitRoutine(RoutineNames.DeathByFear);
            }
            else
            {
                var value = Math.Ceiling(timeLeft);
                if (value != lastKnownValue)
                {
                    timeText.Text = value.ToString(CultureInfo.InvariantCulture);
                    lastKnownValue = value;
                    if (lastKnownValue > 15)
                        timeText.Color = ColorPalette.Text.Highlight;

                    else if (lastKnownValue > 5)
                        timeText.Color = ColorPalette.Text.Orange;

                    else
                        timeText.Color = ColorPalette.Text.Red;
                }
            }
        }

        #endregion

        // IsRunning
        public bool IsRunning { get; private set; }

        // Reset
        public void Reset()
        {
            timeLeft = 0;
            IsRunning = false;
        }

        // Start
        public void Start(int duration)
        {
            if (duration < 0)
            {
                Reset();
                return;
            }

            lastKnownValue = -1;
            timeLeft = duration;
            IsRunning = true;
        }
    }
}
