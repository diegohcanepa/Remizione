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
            this.GlobalLight = new("GlobalLight")
            {
                //Color = ColorPalette.GlobalLight.Default,
                LightKind = LightKind.Global,               
                PivotOrigin = RectanglePoint.Center,
                Position = Screen.Center,
                Scale = new(18,10)
            };

            GlobalLight.TurnOn();
        }

        // GlobalLight
        public Light GlobalLight { get; }
    }
}
