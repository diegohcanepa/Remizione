using Engendro;
using Microsoft.Xna.Framework;
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

            Container.Add(this);
        }

        #endregion

        // Container
        public static DataContainer<RunModifierDefinition> Container { get; } = new(element => new RunModifierDefinition(element));

        // Cooldown
        public int Cooldown { get; }

        // Image
        public AtlasImage Image { get; }

        // MeterBackColor
        public Color MeterBackColor { get; }

        // MeterForeColor
        public Color MeterForeColor { get; }

        // Scope
        public RunModifierScope Scope { get; }
    }
}
