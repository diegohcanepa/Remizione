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

            this.LargeHand = new();
        }

        #region Internal members

        // GlobalLight
        internal Light GlobalLight { get; }

        // LargeHand
        internal LargeHand LargeHand { get; }

        #endregion
    }
}
