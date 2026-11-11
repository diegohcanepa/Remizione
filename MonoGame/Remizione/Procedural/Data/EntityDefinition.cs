using Engendro.Collections;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// EntityDefinition
    /// </summary>
    public abstract class EntityDefinition : Definition
    {
        private static readonly Dictionary<string, EntityDefinition> definitions = [];

        #region Constructor

        // Constructor
        protected EntityDefinition(JsonElement element)
            : base(element)
        {
            // Pools
            Pools = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "pools");

            // Tags
            Tags = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "tags");
        }

        #endregion

        #region Protected members

        // ValidateNameReferences
        protected void ValidateNameReferences(string properyName, IList<string> names)
        {
            for (var i = 0; i < names.Count; i++)
            {
                if (!definitions.ContainsKey(names[i]))
                    throw new InvalidOperationException($"'{names[i]}' listed in [{Name}.{properyName}] does not exist.");
            }
        }

        #endregion

        // Pools
        public ReadOnlyEnumSet<Tag> Pools { get; }

        // Tags
        public ReadOnlyEnumSet<Tag> Tags { get; }
    }
}
