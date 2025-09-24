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
            this.Lightning = new(session);
            this.Rain = new Rain(session);

            // Global light
            this.GlobalLight ??= new Light(session.Game, "GlobalLight")
            {
                Color = ColorPalette.GlobalLight.Default,
                LightKind = LightKind.Global,
                ImageName = "GlobalLight",
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
            };

            this.GlobalLight.Prepare(Atlases.Environment);
        }

        #region Internal members

        // GlobalLight
        internal Light GlobalLight { get; }

        // Update
        internal void Update(GameTime gameTime)
        {
            Lightning.Update(gameTime);
            Rain.Update(gameTime);
        }

        #endregion

        // EnterRoom
        public void EnterRoom()
        {
            Rain.EnterRoom();
        }

        // ExitRoom
        public void ExitRoom()
        {
        }

        // Lightning
        public Lightning Lightning { get; }

        // Rain
        public Rain Rain { get; }
    }
}
