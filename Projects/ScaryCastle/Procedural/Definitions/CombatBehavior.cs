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
    public sealed class CombatBehavior
    {
        #region Private fields

        private static readonly Dictionary<string, CombatBehavior> data = [];
        private static readonly List<CombatBehavior> dataList = [];
        private readonly List<CombatIntentDescriptor> intentDescriptors = [];

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
                    intentDescriptors.Add(new(Name, intentJson));
                }
            }

            IntentDescriptors = intentDescriptors.AsReadOnly();

            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<CombatBehavior> All { get; } = new(dataList);

        // Find
        public static CombatBehavior? Find(string name)
        {
            return data.TryGetValue(name, out var result) ? result : null;
        }

        // Get
        public static CombatBehavior Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"{nameof(CombatBehavior)} '{name}' not found.");
        }

        // Load
        public static void Load(string fileName)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            Utils.LoadJsonData(fileName, element => new CombatBehavior(element));
        }

        #endregion

        // Archetype
        public CombatBehaviorArchetype Archetype { get; }

        // IntentDescriptors
        public ReadOnlyCollection<CombatIntentDescriptor> IntentDescriptors { get; }

        // Name
        public string Name { get; }
    }
}
