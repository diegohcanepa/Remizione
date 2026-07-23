using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// RunModifierDefinition
    /// </summary>
    public sealed class RunModifierDefinition : Definition
    {
        #region Constructor

        // Constructor
        public RunModifierDefinition(JsonElement element)
            : base(element)
        {
            // Cooldown
            this.Cooldown = element.GetInt32("cooldown", 0);

            // Image
            this.Image = Atlases.UI.GetImage($"RunModifier{Name}Icon");

            // MeterBackColor
            this.MeterBackColor = element.GetColor("meterBackColor", Color.White);

            // MeterForeColor
            this.MeterForeColor = element.GetColor("meterForeColor", Color.White);

            // Scope
            this.Scope = element.GetEnum("scope", RunModifierScope.Room);

            Definitions.Add(this);
        }

        #endregion

        // MeterBackColor
        public Color MeterBackColor { get; }

        // MeterForeColor
        public Color MeterForeColor { get; }

        // Cooldown
        public int Cooldown { get; }

        // Definitions
        public static DataContainer<RunModifierDefinition> Definitions { get; } = new(element => new RunModifierDefinition(element));

        // Image
        public AtlasImage Image { get; }

        // Scope
        public RunModifierScope Scope { get; }
    }
}
