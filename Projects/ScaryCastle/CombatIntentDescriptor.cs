using Engendro;
using System;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CombatIntentDescriptor
    /// </summary>
    public sealed class CombatIntentDescriptor : Definition
    {
        // Constructor
        public CombatIntentDescriptor(string behaviorName, JsonElement element)
            : base(element)
        {
            // Category
            if (element.GetEnum<IntentCategory>("category") is not IntentCategory category)
                throw new InvalidOperationException("Missing category property.");
            else
                this.Category = category;

            // LocalizedPhrase
            this.LocalizedPhrase = TextRepository.GetValue($"CombatIntent.{behaviorName}.{Name}");
        }

        // Category
        public IntentCategory Category { get; }

        // LocalizedPhrase
        public string LocalizedPhrase { get; }
    }
}