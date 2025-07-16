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
            if (session.GameplayMode == GameplayMode.Survival)
            {
                Lightning.Update(gameTime);
                Rain.Update(gameTime);
            }
        }

        #endregion

        // EnterRoom
        public void EnterRoom(GameRoom room)
        {
            Rain.EnterRoom();
        }

        // GlobalLightColor
        public Color GlobalLightColor => Color.White;

        // Lightning
        public Lightning Lightning { get; }

        // Rain
        public Rain Rain { get; }
    }
}
