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
            : base(element)
        {
            // Category
            if (element.GetEnum<CombatIntentCategory>("category") is not CombatIntentCategory category)
                throw new InvalidOperationException("Missing category property.");
            else
                this.Category = category;

            // RangeAttack
            RangeAttack = element.GetBool("rangeAttack", false);

            // Sound
            Sound = element.GetObject("sound", Sound.Get);
        }

        // Category
        public CombatIntentCategory Category { get; }

        // RangeAttack
        public bool RangeAttack { get; }

        // Sound
        public Sound? Sound { get; }
    }
}