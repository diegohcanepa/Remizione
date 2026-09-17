using Engendro;

namespace Remizione
{
    /// <summary>
    /// Environment
    /// </summary>
    public sealed class Environment
    {
        // Constructor
        public Environment()
        {
            // Global light
            this.GlobalLight = new("GlobalLight", LightKind.Global)
            {
                Passes = 2,
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
                Scale = new(18, 10)
            };

            GlobalLight.Lit();
        }

        // GlobalLight
        public Light GlobalLight { get; }
    }
}
