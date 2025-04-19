using Microsoft.Xna.Framework;
using System;

namespace Engendro
{
    /// <summary>
    /// Timer
    /// </summary>
    public sealed class Timer
    {
        private int duration;

        #region Constructors

        // Constructor
        public Timer()
            : this(null)
        {
        }

        // Constructor
        public Timer(Action? onComplete)
        {
            this.OnComplete = onComplete;
        }

        #endregion

        // Duration
        public int Duration
        {
            get => duration;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                this.duration = value;

                Stop();
            }
        }

        // ElapsedTime
        public int ElapsedTime { get; private set; }

        // IsRunning
        public bool IsRunning { get; private set; }

        // OnComplete
        public Action? OnComplete { get; set; }

        // Restart
        public void Restart()
        {
            Start(Duration);
        }

        // Start
        public void Start(int duration)
        {
            Stop();

            if (duration <= 0)
            {
                return;
            }

            this.Duration = duration;

            IsRunning = true;
        }

        // StartNew
        public static Timer StartNew(int duration)
        {
            Timer result = new();
            result.Start(duration);
            return result;
        }

        // Stop
        public void Stop()
        {
            Stop(StopBehavior.AsIs);
        }

        // Stop
        public void Stop(StopBehavior stopBehavior)
        {
            IsRunning = false;
            ElapsedTime = 0;
            if (stopBehavior == StopBehavior.ForceComplete)
            {
                OnComplete?.Invoke();
            }
        }

        // Update
        public bool Update(GameTime gameTime)
        {
            if (!IsRunning)
            {
                return false;
            }

            if (ElapsedTime < Duration)
            {
                ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (ElapsedTime >= Duration)
                {
                    Stop(StopBehavior.ForceComplete);
                    return false;
                }
            }

            return true;
        }
    }
}
