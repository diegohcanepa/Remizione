using Engendro;
using Engendro.Audio;
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
        #region Private fields

        private SoundInstance? alarmSoundInstance;
        private readonly Vector2 defaultTextSize = ScaleInfo.Text.Galactus;
        private readonly Sprite icon = new(Atlases.UI.CountdownSkullIcon) { PivotOrigin = RectanglePoint.RightTop, Scale = new(.8f) };
        private double lastKnownValue;
        private readonly Vector2Tween scaleTween = new();
        private readonly GameSession session;
        private float timeLeft;
        private readonly TextSprite timeText;

        #endregion

        // Constructor
        public UICountdown(GameSession session)
        {
            this.session = session;

            icon.Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, 0, -2);

            // Time text
            this.timeText = new TextSprite(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -(7 + icon.BoundingBox.Width), 5),
                Scale = defaultTextSize,
                Spacing = -6,
            };
        }

        #region Private members

        // StartCriticalPhase
        private void StartCriticalPhase()
        {
            if (alarmSoundInstance != null)
                return;

            timeText.Color = ColorPalette.Text.Red;

            this.alarmSoundInstance = Sound.Get(SoundNames.Alarm).PopInstance();
            if (alarmSoundInstance != null)
            {
                alarmSoundInstance.TransitionAware = false;
                alarmSoundInstance.IsLooped = true;
                alarmSoundInstance.Play();
            }

            scaleTween.Start(TweenStyle.Linear, defaultTextSize, defaultTextSize * 1.1f, 250, -1);
            timeText.Tweens.ScaleTween = scaleTween;

            session.TextHUD.Message.Show(MessageKind.BackToHallway);
        }

        // StopCriticalPhase
        private void StopCriticalPhase()
        {
            if (alarmSoundInstance != null)
            {
                alarmSoundInstance?.Stop();
                alarmSoundInstance = null;
            }

            timeText.Color = ColorPalette.Text.Highlight;
            scaleTween.Stop();
            timeText.Scale = defaultTextSize;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (IsRunning)
            {
                icon.Draw(gameTime);
                timeText.Draw(gameTime);
            }
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            if (session.IsAwaiting || !IsRunning || timeLeft <= 0)
                return;

            timeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                IsRunning = false;
                IsExpired = true;
                StopCriticalPhase();
                session.Player?.StopMoving();
                session.AwaitRoutine(session.Room is CorridorRoom ? RoutineNames.DeathByMandinga : RoutineNames.DeathByCorridorLever);
            }
            else
            {
                var value = Math.Ceiling(timeLeft);

                if (value != lastKnownValue)
                {
                    timeText.Text = value.ToString("00", CultureInfo.InvariantCulture);
                    lastKnownValue = value;
                }

                if (value < GameSettings.CountdownCritical)
                    StartCriticalPhase();
                else
                    StopCriticalPhase();
            }

            timeText.Update(gameTime);
        }


        #endregion

        // IsCritical
        public bool IsCritical => alarmSoundInstance != null;

        // IsExpired
        public bool IsExpired { get; private set; }

        // IsRunning
        public bool IsRunning { get; private set; }

        // Reset
        public void Reset()
        {
            timeLeft = 0;
            lastKnownValue = -1;
            StopCriticalPhase();
            IsRunning = false;
            IsExpired = false;
        }

        // Start
        public void Start()
        {
            Reset();

            this.timeLeft = GameSettings.CountdownDuration;
            this.timeText.Text = timeLeft.ToString(CultureInfo.InvariantCulture);
            this.timeText.Color = ColorPalette.Text.Highlight;
            this.IsRunning = true;
        }
    }
}
