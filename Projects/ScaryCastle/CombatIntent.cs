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
            // Category
            Category = element.GetEnum<CombatIntentCategory>("category", CombatIntentCategory.Basic);

            // Sound
            Sound = element.GetObject("sound", Sound.Get);

            // ThrownObject
            ThrownObject = element.GetEnum<ThrownObjectType>("thrownObject", ThrownObjectType.None);
        }

        // Category
        public CombatIntentCategory Category { get; }

        // Sound
        public Sound? Sound { get; }

        // ThrownObject
        public ThrownObjectType ThrownObject { get; }
    }
}