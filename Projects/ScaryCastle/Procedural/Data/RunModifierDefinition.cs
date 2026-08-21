using Engendro;
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

            // Scope
            this.Scope = element.GetEnum("scope", RunModifierScope.Room);
        }

        #endregion

        // Cooldown
        public int Cooldown { get; }

        // Data
        public static DataContainer<RunModifierDefinition> Data { get; } = new(element => new(element));

        // Image
        public AtlasImage Image { get; }

        // Scope
        public RunModifierScope Scope { get; }
    }
}
