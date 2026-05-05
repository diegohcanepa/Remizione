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
        private SoundInstance? alarmSoundInstance;
        private readonly Vector2 defaultTextSize = ScaleInfo.Text.Galactus;
        private double lastKnownValue;
        private readonly Vector2Tween scaleTween = new();
        private readonly GameSession session;
        private float timeLeft;
        private readonly TextSprite timeText;

        // Constructor
        public UICountdown(GameSession session)
        {
            this.session = session;

            // Time text
            this.timeText = new TextSprite(Fonts.CommonOutline)
            {
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.HUDArea.GetPoint(RectanglePoint.RightTop, -7, 4),
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
        }

        // StopCriticalPhase
        private void StopCriticalPhase()
        {
            if (alarmSoundInstance != null)
            {
                alarmSoundInstance?.Stop(2000);
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
            if (session.IsAwaiting || !IsRunning)
                return;

            timeText.Draw(gameTime);
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
                session.AwaitRoutine(RoutineNames.DeathByTime);
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

        // IsRunning
        public bool IsRunning { get; private set; }

        // Reset
        public void Reset()
        {
            timeText.Color = ColorPalette.Text.Highlight;
            timeLeft = 0;
            IsRunning = false;
            StopCriticalPhase();
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
