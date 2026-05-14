using Engendro;

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
            // Global light
            this.GlobalLight ??= new("GlobalLight")
            {
                Color = ColorPalette.GlobalLight.Default,
                LightKind = LightKind.Global,
                ImageName = "GlobalLight",
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
            };

            this.GlobalLight.Prepare(Atlases.Environment);

            this.DevilHand = new(DeityHandKind.Devil);
            this.GodHand = new(DeityHandKind.God);
        }

        #region Internal members

        // DevilHand
        internal DeityHand DevilHand { get; }

        // GlobalLight
        internal Light GlobalLight { get; }

        // GodHand
        internal DeityHand GodHand { get; }

        #endregion
    }
}
