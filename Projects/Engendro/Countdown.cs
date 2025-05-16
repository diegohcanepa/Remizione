using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Countdown
    /// </summary>
    public sealed class Countdown
    {
        private int restartDuration;

        // IsRunning
        public bool IsRunning => TimeLeft > 0;

        // Restart
        public void Restart()
        {
            if (restartDuration > 0)
            {
                Start(restartDuration);
            }
        }

        // Start
        public void Start(int duration)
        {
            restartDuration = duration;
            TimeLeft = Math.Max(0, duration);
        }

        // StartFromRange
        public void StartFromRange()
        {
            Start(TimeRange.Random());
        }

        // Stop
        public void Stop()
        {
            TimeLeft = 0;
        }

        // TimeLeft
        public int TimeLeft { get; private set; }

        // TimeRange
        public Int32Range TimeRange { get; set; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (IsRunning)
            {
                TimeLeft -= gameTime.ElapsedGameTime.Milliseconds;

                if (TimeLeft <= 0)
                    TimeLeft = 0;
            }
        }
    }
}
