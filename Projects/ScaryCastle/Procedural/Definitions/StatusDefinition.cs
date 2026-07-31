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

            // Scope
            this.Scope = element.GetEnum("scope", RunModifierScope.Room);

            // Image
            this.Image = Atlases.UI.GetImage($"Status{StatusType}Icon");

            Container.Add(this);
        }

        #endregion

        // Container
        public static DataContainer<StatusDefinition> Container { get; } = new(element => new StatusDefinition(element));

        // Image
        public AtlasImage Image { get; }

        // MeterBackColor
        public Color MeterBackColor { get; }

        // MeterForeColor
        public Color MeterForeColor { get; }

        // Scope
        public RunModifierScope Scope { get; }

        // StatusType
        public StatusType StatusType { get; }
    }
}
