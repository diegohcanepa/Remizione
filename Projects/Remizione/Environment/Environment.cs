using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        private readonly GameSession session;

        // Constructor
        public Environment(GameSession session)
        {
            this.session = session;
            this.Rain = new Rain(session);
        }

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            if (CycleCooldown > 0)
            {
                CycleCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                if (CycleCooldown <= 0)
                    BeginCycle(CurrentCycle == Cycle.Indulgence ? Cycle.Penance : Cycle.Indulgence);
            }

            Rain.Update(gameTime);
        }

        #endregion

        // BeginCycle
        public void BeginCycle(Cycle cycle)
        {
            CurrentCycle = cycle;
            CycleCooldown = Randomizer.Next(180_000, 300_000);
            session.CycleCount++;
        }

        // EnterRoom
        public void EnterRoom(GameRoom room)
        {
            Rain.EnterRoom();
        }

        // Cycle
        public Cycle CurrentCycle { get; private set; }

        // CycleCooldown
        public int CycleCooldown { get; set; }

        // Rain
        public Rain Rain { get; }
    }
}
