using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Engendro
{
    /// <summary>
    /// Blinker
    /// </summary>
    public class Blinker<T> where T : struct
    {
        #region Private fields

        private int counter;
        private float elapsedInterval;
        private int valueIndex;
        private readonly List<T> values;

        #endregion

        // Constructor
        public Blinker(params T[] values)
        {
            if (values.Length < 2)
                throw new InvalidOperationException("You must specify at least two values.");

            this.values = [.. values];
        }

        // Count
        public int Count { get; private set; }

        // CurrentValue
        public T CurrentValue => values[valueIndex];

        // Interval
        public float Interval { get; private set; }

        // IsRunning
        public bool IsRunning { get; private set; }

        // Reset
        public void Reset()
        {
            valueIndex = 0;
            IsRunning = false;
            Count = 0;
        }

        // Restart
        public void Restart()
        {
            Stop();
            Start(Interval, Count);
        }

        // Start
        public void Start(float interval)
        {
            Start(interval, -1);
        }

        // Start
        public void Start(float interval, int count)
        {
            if (interval <= 0f)
            {
                Stop();
                return;
            }

            this.valueIndex = 0;
            this.Interval = interval;
            this.Count = count;
            this.counter = 0;
            this.elapsedInterval = interval;

            IsRunning = true;
        }

        // Stop
        public void Stop()
        {
            Reset();
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!IsRunning)
                return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            elapsedInterval -= delta;

            while (elapsedInterval <= 0f && IsRunning)
            {
                valueIndex++;
                if (valueIndex == values.Count)
                {
                    valueIndex = 0;
                    if (Count > 0)
                        counter++;
                }

                if (Count > 0 && counter >= Count)
                {
                    Stop();
                    break;
                }

                elapsedInterval += Interval;
            }
        }
    }
}