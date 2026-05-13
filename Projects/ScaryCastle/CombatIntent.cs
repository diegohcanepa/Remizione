using Engendro;
using Engendro.Audio;
using System;
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
            this.Contact = string.Equals(Name, nameof(Contact), StringComparison.OrdinalIgnoreCase);
            this.Range = element.GetInt32("range", 5);
            this.Sound = element.GetObject("sound", Sound.Get);
        }

        // Category
        public CombatIntentCategory Category { get; }

        // Contact
        public bool Contact { get; }

        // Range
        public int Range { get; }

        // Sound
        public Sound? Sound { get; }
    }
}