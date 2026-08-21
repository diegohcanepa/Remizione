using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// StatusDefinition
    /// </summary>
    public sealed class StatusDefinition : Definition
    {
        #region Constructor

        // Constructor
        public StatusDefinition(JsonElement element)
            : base(element)
        {
            // StatusType
            this.StatusType = Enum.Parse<StatusType>(Name);

            // MeterBackColor
            this.MeterBackColor = element.GetColor("meterBackColor", Color.White);

            // MeterForeColor
            this.MeterForeColor = element.GetColor("meterForeColor", Color.White);

            // Image
            this.Image = Atlases.UI.GetImage($"Status{StatusType}Icon");
        }

        #endregion

        // Data
        public static DataContainer<StatusDefinition> Data { get; } = new(element => new(element));

        // Image
        public AtlasImage Image { get; }

        // MeterBackColor
        public Color MeterBackColor { get; }

        // MeterForeColor
        public Color MeterForeColor { get; }

        // StatusType
        public StatusType StatusType { get; }
    }
}
