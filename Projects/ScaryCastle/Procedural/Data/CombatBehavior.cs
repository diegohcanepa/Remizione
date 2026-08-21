using Engendro;
using System;
using System.Collections.Generic;
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
            if (element.GetEnum<CombatArchetypeName>("archetype") is not CombatArchetypeName archetype)
                throw new InvalidOperationException("Missing archetype property.");
            else
                this.Archetype = Archetypes.Get(archetype);

            // Intents
            if (element.TryGetProperty("intents", out JsonElement intentsArray))
            {
                foreach (var intentJson in intentsArray.EnumerateArray())
                {
                    intents.Add(new(intentJson));
                }
            }

            // TurnInterval
            TurnInterval = element.GetInt32("turnInterval", 2);

            Intents = new(intents);
        }

        #endregion

        // Archetype
        public CombatArchetype Archetype { get; }

        // Data
        public static DataContainer<CombatBehavior> Data { get; } = new(element => new CombatBehavior(element));

        // DefaultIntent
        public CombatIntent? DefaultIntent { get; }

        // Intents
        public NamedObjectReadOnlyCollection<CombatIntent> Intents { get; }

        // Name
        public string Name { get; }

        // TurnInterval
        public int TurnInterval { get; }
    }
}
