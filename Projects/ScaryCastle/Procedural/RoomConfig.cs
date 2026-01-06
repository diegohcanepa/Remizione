using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// RoomConfig
    /// </summary>
    public sealed class RoomConfig : Config
    {
        private static readonly Dictionary<string, RoomConfig> data = [];
        private static readonly List<RoomConfig> dataList = [];

        // Constructor
        private RoomConfig(JsonElement element)
            : base(element)
        {
            // LockType
            if (element.TryGetProperty("lockType", out JsonElement lockTypeElement))
                LockType = Enum.Parse<LockType>(lockTypeElement.GetString() ?? string.Empty);

            // MaxEnemies
            MaxEnemies = -1;
            if (element.TryGetProperty("maxEnemies", out JsonElement maxEnemiesElement))
                MaxEnemies = maxEnemiesElement.GetInt32();

            // MaxProps
            MaxProps = -1;
            if (element.TryGetProperty("maxProps", out JsonElement maxPropsElement))
                MaxProps = maxPropsElement.GetInt32();

            // RequiresDeadEnd
            if (element.TryGetProperty("requiresDeadEnd", out JsonElement requiresDeadEndElement))
                RequiresDeadEnd = requiresDeadEndElement.GetBoolean();

            // Scope
            var allowPools = Tags.FromJson(element, "allowPools");
            var denyPools = Tags.FromJson(element, "denyPools");
            var allowTags = Tags.FromJson(element, "allowTags");
            var denyTags = Tags.FromJson(element, "denyTags");

            this.Scope = new ScopeRules(allowPools, denyPools, allowTags, denyTags);

            // Template
            if (element.TryGetProperty("template", out JsonElement templateElement))
                Template = templateElement.GetString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(Template))
                throw new InvalidOperationException($"Missing template in room config [{Name}].");

            if (AotTypeRegistry.Find(Template) == null)
                throw new InvalidOperationException($"Template '{Template}' is not valid.");

            data.Add(Name, this);
            dataList.Add(this);
        }

        #region Static members

        // All
        public static ReadOnlyCollection<RoomConfig> All { get; } = new(dataList);

        // Find
        public static RoomConfig? Find(string name)
        {
            return data.TryGetValue(name, out var roomConfig) ? roomConfig : null;
        }

        // Get
        public static RoomConfig Get(string name)
        {
            return data[name];
        }

        // Load
        public static void Load(string fileName)
        {
            Utils.LoadJsonData(fileName, element => new RoomConfig(element));
        }

        #endregion

        // LockType
        public LockType LockType { get; }

        // MaxEnemies
        public int MaxEnemies { get; }

        // MaxProps
        public int MaxProps { get; }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // Scope
        public ScopeRules Scope { get; }

        // Template
        public string Template { get; }
    }
}
