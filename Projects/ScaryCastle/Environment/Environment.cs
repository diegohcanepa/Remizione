using Engendro;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        // Constructor
        public Environment(GameSession session)
        {
            this.Lightning = new(session);

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
        }

        #endregion

        // Lightning
        public Lightning Lightning { get; }
    }
}
