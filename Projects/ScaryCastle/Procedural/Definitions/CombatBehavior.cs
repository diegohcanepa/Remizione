using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CombatBehavior
    /// </summary>
    public sealed class CombatBehavior : INamedObject
    {
        #region Private fields

        private readonly List<CombatIntent> intents = [];

        #endregion

        #region Constructor

        // Constructor
        private CombatBehavior(JsonElement element)
        {
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidOperationException("Name not found.");
            CodeContract.ValidName(this.Name, string.Empty);

            // Archetype
            if (element.GetEnum<CombatBehaviorArchetype>("archetype") is not CombatBehaviorArchetype archetype)
                throw new InvalidOperationException("Missing archetype property.");
            else
                this.Archetype = archetype;

            // Intents
            if (element.TryGetProperty("intents", out JsonElement intentsArray))
            {
                foreach (var intentJson in intentsArray.EnumerateArray())
                {
                    intents.Add(new(intentJson));
                }
            }

            IntentDescriptors = new(intents);

            Behaviors.Add(this);
        }

        #endregion

        // Archetype
        public CombatBehaviorArchetype Archetype { get; }

        // Behaviors
        public static DataContainer<CombatBehavior> Behaviors { get; } = new(element => new CombatBehavior(element));

        // IntentDescriptors
        public NamedObjectReadOnlyCollection<CombatIntent> IntentDescriptors { get; }

        // Name
        public string Name { get; }
    }
}
