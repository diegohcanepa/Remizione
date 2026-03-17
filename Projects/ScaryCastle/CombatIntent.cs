using Engendro;
using Engendro.Audio;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CombatIntent
    /// </summary>
    public sealed class CombatIntent : Definition
    {
        // Constructor
        public CombatIntent(JsonElement element)
            : base(element, false)
        {
            this.Category = element.GetEnum("category", CombatIntentCategory.Basic);
            this.Sound = element.GetObject("sound", Sound.Get);
            this.ThrownObject = element.GetEnum("thrownObject", ThrownObjectType.None);
            this.DisplayName = TextRepository.GetValue($"CombatIntent.{Name}");
        }

        // Category
        public CombatIntentCategory Category { get; }

        // DisplayName
        public string DisplayName { get; }

        // Sound
        public Sound? Sound { get; }

        // ThrownObject
        public ThrownObjectType ThrownObject { get; }
    }
}