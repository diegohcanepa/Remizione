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
            this.Owner = owner;
            this.Category = element.GetEnum("category", CombatIntentCategory.Basic);
            this.Sound = element.GetObject("sound", Sound.Get);
            this.ThrownObject = element.GetEnum("thrownObject", ThrownObjectType.None);
            this.DisplayName = TextRepository.GetValue($"{nameof(CombatBehavior)}.{owner.Name}.{Name}");
        }

        // Category
        public CombatIntentCategory Category { get; }

        // DisplayName
        public string DisplayName { get; }

        // Owner
        public CombatBehavior Owner { get; }

        // Sound
        public Sound? Sound { get; }

        // ThrownObject
        public ThrownObjectType ThrownObject { get; }
    }
}