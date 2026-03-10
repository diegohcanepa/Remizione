using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    // FearManager
    // Handles the player's fear logic using discrete integer points.
    public sealed class FearManager
    {
        private double lastFearTickTime;

        #region Private members

        // GetFearInterval
        private int GetFearInterval(bool hasEnemies) => hasEnemies ? 8000 : 30000;

        #endregion

        // CurrentFear
        public int CurrentFear
        {
            get;
            set => field = Math.Clamp(value, 0, MaxFear);
        }

        // Update
        // Updates the fear level based on elapsed total game time.
        public void Update(GameTime gameTime, bool hasEnemies)
        {
            double interval = GetFearInterval(hasEnemies);
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;

            if (currentTime - lastFearTickTime >= interval)
            {
                CurrentFear = Math.Clamp(CurrentFear + 1, 0, MaxFear);
                lastFearTickTime = currentTime;
            }
        }

        // MaxFear
        // The maximum amount of fear the player can reach.
        public int MaxFear { get; } = 10;
    }
}