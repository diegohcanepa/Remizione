using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Countdown
    /// </summary>
    public sealed class Countdown
    {
        private float restartDuration;

        // DefaultDuration
        public float DefaultDuration { get; set; } = 1;

        // IsRunning
        public bool IsRunning => TimeLeft > 0f;

        // Restart
        public void Restart()
        {
            if (restartDuration > 0)
                Start(restartDuration);
        }

        // Start
        public void Start()
        {
            Start(DefaultDuration);
        }

        // Start
        public void Start(float duration)
        {
            restartDuration = duration;
            TimeLeft = MathF.Max(0f, duration);
        }

        // Stop
        public void Stop()
        {
            TimeLeft = 0f;
        }

        // TimeLeft
        public float TimeLeft { get; private set; }

        // Update
        public void Update(GameTime gameTime)
        {
            if (IsRunning)
            {
                TimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (TimeLeft <= 0f)
                    TimeLeft = 0f;
            }
        }
    }
}