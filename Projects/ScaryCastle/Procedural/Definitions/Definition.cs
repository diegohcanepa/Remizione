using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// Definition
    /// </summary>
    public abstract class Definition<T> : INamedObject where T : Definition<T>
    {
        private static readonly Dictionary<string, T> data = [];
        private static readonly List<T> dataList = [];
        private static readonly HashSet<string> definitions = [];
        private readonly List<EffectDescriptor> effectDescriptors = [];

        #region Constructor

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

            // Effects
            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    effectDescriptors.Add(new(effectJson));
                }
            }

            EffectDescriptors = effectDescriptors.AsReadOnly();
        }

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<T> All { get; } = new(dataList);

        // Find
        public static T? Find(string name)
        {
            return data.TryGetValue(name, out var definition) ? definition : null;
        }

        // Get
        public static T Get(string name)
        {
            return data[name];
        }

        // Load
        public static void Load(string fileName)
        {
            Utils.LoadJsonData(fileName, element => new T(element));
        }

        #endregion

        // EffectDescriptors
        public ReadOnlyCollection<EffectDescriptor> EffectDescriptors { get; }

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
