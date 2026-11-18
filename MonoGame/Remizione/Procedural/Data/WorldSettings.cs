using Engendro;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// WorldSettings
    /// </summary>
    public sealed class WorldSettings
    {
        #region Constructor

        // Constructor
        public WorldSettings()
        {
            var fileName = Path.Combine("Content", ContentFolder.System.ToString(), "WorldSettings.json");
            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var element = doc.RootElement;

            // BonfireDuration
            BonfireDuration = element.GetInt32("bonfireDuration", 40);

            // DarkCycleTransitionDuration
            DarkCycleTransitionDuration = element.GetInt32("darkCycleTransitionDuration", 15);

            // DarkCycleDuration
            DarkCycleDuration = element.GetInt32("darkCycleDuration", 90);

            // LightCycleDuration
            LightCycleDuration = element.GetInt32("lightCycleDuration", 180);

            // LightCycleTransitionDuration
            LightCycleTransitionDuration = element.GetInt32("lightCycleTransitionDuration", 15);

            // SulfurStoneDuration
            SulfurStoneDuration = element.GetInt32("sulfurStoneDuration", 40);
        }

        #endregion

        // BonfireDuration
        public int BonfireDuration { get; }

        // DarkCycleTransitionDuration
        public int DarkCycleTransitionDuration { get; }

        // DarkCycleDuration
        public int DarkCycleDuration { get; }

        // LightCycleDuration
        public int LightCycleDuration { get; }

        // LightCycleTransitionDuration
        public int LightCycleTransitionDuration { get; }

        // SulfurStoneDuration
        public int SulfurStoneDuration { get; }
    }
}
