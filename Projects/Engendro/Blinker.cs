using Microsoft.Xna.Framework;
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
        private int delayCooldown;
        private int elapsedInterval;
        private int valueIndex;
        private readonly List<T> values;

        #endregion

        // Constructor
        public Blinker(params T[] values)
        {
            this.values = new(values);
        }

        // Count
        public int Count { get; private set; }

        // CurrentValue
        public T CurrentValue => values[valueIndex];

        // InDelayPhase
        public bool InDelayPhase => delayCooldown > 0;

        // Interval
        public Int32Range Interval { get; private set; }

        // IsRunning
        public bool IsRunning { get; private set; }

        // Restart
        public void Restart()
        {
            Stop();
            Start(Interval, Count);
        }

        // Start
        public void Start(int interval)
        {
            Start(interval, -1);
        }

        // Start
        public void Start(int interval, int count)
        {
            Start(new Int32Range(interval), count);
        }

        // Start
        public void Start(Int32Range interval)
        {
            Start(interval, -1);
        }

        // Start
        public void Start(Int32Range interval, int count)
        {
            if (interval.Maximum < 1)
            {
                Stop();
                return;
            }

            this.valueIndex = 0;
            this.Interval = interval;
            this.Count = count;
            this.counter = 0;
            this.delayCooldown = StartDelay;
            this.elapsedInterval = interval.Random();

            IsRunning = true;
        }

        // StartDelay
        public int StartDelay { get; set; }

        // Stop
        public void Stop()
        {
            valueIndex = 0;
            IsRunning = false;
            delayCooldown = 0;
            Count = 0;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (!IsRunning)
                return;

            if (delayCooldown > 0)
            {
                delayCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            if (elapsedInterval > 0)
            {
                elapsedInterval -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                if (Count > 0)
                    counter++;

                if (Count > 0 && Count == counter)
                {
                    Stop();
                }
                else
                {
                    this.elapsedInterval = Interval.Random();
                    valueIndex++;
                    if (valueIndex == values.Count)
                        valueIndex = 0;
                }
            }
        }
    }
}
