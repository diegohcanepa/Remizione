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
            this.Lightning = new(session);
            this.Rain = new Rain(session);
        }

        #region Private members

        // LerpColorCubicIn
        private Color LerpColorCubicIn(Color a, Color b, float t)
        {
            t = t * t * t; // easing Cubic In

            byte r = (byte)(a.R + (b.R - a.R) * t);
            byte g = (byte)(a.G + (b.G - a.G) * t);
            byte bVal = (byte)(a.B + (b.B - a.B) * t);
            byte aVal = (byte)(a.A + (b.A - a.A) * t);

            return new Color(r, g, bVal, aVal);
        }

        #endregion

        #region Internal members

        // Update
        internal void Update(GameTime gameTime)
        {
            if (session.PurgatoryMode)
            {
                Lightning.Update(gameTime);

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

        // GlobalLightColor
        public Color GlobalLightColor
        {
            get
            {
                if (Cycle == Cycle.Indulgence)
                    return LerpColorCubicIn(Color.White, Color.IndianRed, 1 - ((float)CycleCooldown / GameSettings.CycleDuration));
                else
                    return Color.IndianRed;
            }
        }

        // Lightning
        public Lightning Lightning { get; }

        // Rain
        public Rain Rain { get; }
    }
}
