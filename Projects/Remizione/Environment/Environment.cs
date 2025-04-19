using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        // BeginCycle
        public void BeginCycle(Cycle cycle)
        {
            CurrentCycle = cycle;
            CycleCooldown = Randomizer.Next(180_000, 300_000);
        }

        // Update
        internal void Update(GameTime gameTime)
        {
            if (CycleCooldown > 0)
            {
                CycleCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (CycleCooldown <= 0)
                    BeginCycle(CurrentCycle == Cycle.Indulgence ? Cycle.Penance : Cycle.Indulgence);
            }
        }

        // Cycle
        public Cycle CurrentCycle { get; private set; }

        // CycleCooldown
        public int CycleCooldown { get; set; }

    }
}
