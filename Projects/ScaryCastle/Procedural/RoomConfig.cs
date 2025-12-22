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

            if (!RideRoom.IsRegistered(Template))
                throw new InvalidOperationException($"Template '{Template}' is not valid.");

            // Placeholders override
            var placeholdersList = new List<PlaceholderOverride>();
            if (element.TryGetProperty("placeholders", out JsonElement placeholdersElement) && placeholdersElement.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement placeholderElement in placeholdersElement.EnumerateArray())
                {
                    // Name
                    var phName = placeholderElement.GetProperty("name").GetString() ?? throw new InvalidOperationException("Placeholder must have a name.");

                    // Check if template has a placeholder
                    if (!RideRoom.HasPlaceholder(Template, phName))
                        throw new InvalidOperationException($"There is no {phName} placeholder defined in template [{Template}]");

                    // Fill chance
                    float? phFillChance = null;
                    if (element.TryGetProperty("fillChance", out JsonElement fillChanceElement))
                        phFillChance = fillChanceElement.GetSingle();

                    // Target
                    PlaceholderTarget? phTarget = null;
                    if (placeholderElement.TryGetProperty("target", out JsonElement targetElement))
                    {
                        if (Enum.TryParse(targetElement.GetString(), out PlaceholderTarget placeholderTarget))
                            phTarget = placeholderTarget;
                    }

                    // AllowTags
                    Tags? phAllowTags = Tags.FromJson(placeholderElement, "allowTags");
                    if (phAllowTags.Count == 0)
                        phAllowTags = null;

                    // 4. Crear la instancia de Placeholder
                    placeholdersList.Add(new PlaceholderOverride(phName, phFillChance, phAllowTags, phTarget));
                }
            }

            this.PlaceholderOverrides = new ReadOnlyCollection<PlaceholderOverride>(placeholdersList);

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

        // FindNotNull
        public static RoomConfig FindNotNull(string name)
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

        // PlaceholderOverrides
        public ReadOnlyCollection<PlaceholderOverride> PlaceholderOverrides { get; }

        // Scope
        public ScopeRules Scope { get; }

        // Template
        public string Template { get; }
    }
}
