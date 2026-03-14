using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    // FearManager
    // Handles the player's fear logic using discrete integer points.
    public sealed class FearManager
    {
        private const double interval = 20000;
        private double lastFearTickTime;

        // CurrentValue
        public int CurrentValue
        {
            get;
            set => field = Math.Clamp(value, 0, MaximumValue);
        }

        // Update
        // Updates the fear level based on elapsed total game time.
        public void Update(GameTime gameTime)
        {
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

            if (currentTime - lastFearTickTime >= interval)
            {
                CurrentValue = Math.Clamp(CurrentValue + 1, 0, MaximumValue);
                lastFearTickTime = currentTime;
            }
        }

        // MaximumValue
        // The maximum amount of fear the player can reach.
        public int MaximumValue { get; } = 100;
    }
}