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
        public CombatIntent(CombatBehavior owner, JsonElement element)
            : base(element, false)
        {
            this.Category = element.GetEnum("category", CombatIntentCategory.Basic);
            this.Range = element.GetInt32("range", 5);
            this.Sound = element.GetObject("sound", Sound.Get);
            this.DisplayName = TextRepository.GetValue($"{nameof(CombatBehavior)}.{owner.Name}.{Name}");
        }

        // Category
        public CombatIntentCategory Category { get; }

        // DisplayName
        public string DisplayName { get; }

        // Range
        public int Range { get; }

        // Sound
        public Sound? Sound { get; }
    }
}