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

            // DarkCycleDuration
            DarkCycleDuration = element.GetInt32("darkCycleDuration", 90);

            // LightCycleDuration
            LightCycleDuration = element.GetInt32("lightCycleDuration", 180);
        }

        #endregion

        // DarkCycleDuration
        public int DarkCycleDuration { get; }

        // LightCycleDuration
        public int LightCycleDuration { get; }
    }
}
