using Engendro;
using Engendro.Collections;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Remizione
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
        public CombatBehavior(JsonElement element)
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

            // LeashRadius
            LeashRadius = element.GetInt32("leashRadius", 0);

            Intents = new(intents);
        }

        #endregion

        // Archetype
        public CombatArchetype Archetype { get; }

        // DefaultIntent
        public CombatIntent? DefaultIntent { get; }

        // Intents
        public NamedReadOnlyCollection<CombatIntent> Intents { get; }

        // LeashRadius
        // Radio máximo de dispersión permitido alrededor de la HomePosition.
        // Es nómade si es <= 0
        public float LeashRadius { get; }

        // Name
        public string Name { get; }
    }
}
