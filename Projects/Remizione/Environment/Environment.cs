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
            if ((session.PurgatoryMode))
            {
                if (CycleCooldown > 0)
                {
                    CycleCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                    if (CycleCooldown <= 0)
                    {
                        if (Cycle == Cycle.Indulgence)
                            Cycle = Cycle.Penance;
                        else
                            Cycle = Cycle.Indulgence;

                        CycleCooldown = GameSettings.CycleDuration;
                        CycleCount++;
                    }
                }

                Rain.Update(gameTime);
            }
        }

        #endregion

        // EnterRoom
        public void EnterRoom(GameRoom room)
        {
            Rain.EnterRoom();
        }

        // Cycle
        public Cycle Cycle { get; set; } = Cycle.Indulgence;

        // CycleCooldown
        public int CycleCooldown { get; set; } = GameSettings.CycleDuration;

        // CycleCount
        public int CycleCount { get; set; } = 1;

        // Rain
        public Rain Rain { get; }
    }
}
