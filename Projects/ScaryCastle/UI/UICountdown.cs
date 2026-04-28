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
        private readonly TextSprite labelText;
        private double lastKnownValue;
        private readonly GameSession session;
        private float timeLeft;
        private readonly TextSprite timeText;

        // Constructor
        public UICountdown(GameSession session)
        {
            this.session = session;

            // Label text
            this.labelText = new TextSprite(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Bottom,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.Bottom, 0, -3),
                Scale = ScaleInfo.Text.Huge,
                Spacing = -6,
                Text = TextRepository.GetValue("Misc.Escape")
            };

            // Time text
            this.timeText = new TextSprite(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Left,
                Position = labelText.BoundingBox.GetPoint(RectanglePoint.Right, 2, 0),
                Scale = ScaleInfo.Text.Huge,
                Spacing = -6,
            };

            // Icon
            this.icon = new(Atlases.UI.SkullIcon)
            {
                PivotOrigin = RectanglePoint.Right,
                Position = labelText.BoundingBox.GetPoint(RectanglePoint.Left, -1, -.75f)
            };
        }

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (!IsRunning)
                return;

            icon.Draw(gameTime);
            labelText.Draw(gameTime);
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

                    labelText.Color = timeText.Color;
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
