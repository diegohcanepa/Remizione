using Engendro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// Definition
    /// </summary>
    public abstract class Definition : INamedObject
    {
        private static readonly HashSet<string> definitions = [];

        // Constructor
        protected Definition(JsonElement element)
        {
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);

            // Name cannot be a realm 
            if (Enum.IsDefined(typeof(Realm), Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is an item realm.");

            // Name cannot be a category
            if (Enum.IsDefined(typeof(ItemCategory), Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is an item category.");

            if (definitions.Contains(Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is already being used by another definition.");
            else
                definitions.Add(Name);

            // SpawnWeight
            SpawnWeight = element.GetFloat("spawnWeight", 1);
        }

        // Name
        public string Name { get; }

        // SpawnWeight
        public Ratio SpawnWeight { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
