using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// RoomDefinition
    /// </summary>
    public sealed class RoomDefinition : EntityDefinition
    {
        private static readonly Dictionary<string, RoomDefinition> data = [];
        private static readonly List<RoomDefinition> dataList = [];
        private readonly List<Placeholder> placeholders = [];

        // Constructor
        private RoomDefinition(JsonElement element)
            : base(element)
        {
            // DoorDown
            if (element.TryGetProperty("doorDown", out JsonElement doorDownElement))
                DoorDown = DataConvert.ToVector2(doorDownElement.GetString() ?? string.Empty);

            // DoorLeft
            if (element.TryGetProperty("doorLeft", out JsonElement doorLeftElement))
                DoorLeft = DataConvert.ToVector2(doorLeftElement.GetString() ?? string.Empty);

            // DoorRight
            if (element.TryGetProperty("doorRight", out JsonElement doorRightElement))
                DoorRight = DataConvert.ToVector2(doorRightElement.GetString() ?? string.Empty);

            // DoorUp
            if (element.TryGetProperty("doorUp", out JsonElement doorUpElement))
                DoorUp = DataConvert.ToVector2(doorUpElement.GetString() ?? string.Empty);

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

            // Placeholders
            if (element.TryGetProperty("placeholders", out JsonElement placeholdersElement))
            {
                foreach (var item in placeholdersElement.EnumerateArray())
                {
                    var position = DataConvert.ToVector2(item.GetProperty("position").GetString() ?? string.Empty);

                    // Enum Placement
                    var placementStr = item.GetProperty("placement").GetString() ?? string.Empty;
                    var placement = Enum.Parse<PlacementType>(placementStr);

                    // Ratio FillChance (por defecto 1.0 si no existe)
                    float chanceValue = 1f;
                    if (item.TryGetProperty("fillChance", out JsonElement chanceElement))
                        chanceValue = chanceElement.GetSingle();

                    Ratio fillChance = chanceValue;

                    // Enum Target (por defecto Prop si no existe)
                    var target = PlaceholderTarget.Prop;
                    if (item.TryGetProperty("target", out JsonElement targetElement))
                        target = Enum.Parse<PlaceholderTarget>(targetElement.GetString() ?? string.Empty);

                    placeholders.Add(new Placeholder(position, placement, fillChance, target));
                }
            }

            // RequiresDeadEnd
            if (element.TryGetProperty("requiresDeadEnd", out JsonElement requiresDeadEndElement))
                RequiresDeadEnd = requiresDeadEndElement.GetBoolean();

            // RoomType
            RoomType = RoomType.Connector;
            if (element.TryGetProperty("roomType", out JsonElement roomTypeElement))
                RoomType = Enum.Parse<RoomType>(roomTypeElement.GetString() ?? string.Empty);

            // Scope
            this.Scope = ScopeRules.FromJson(element);

            // WalkArea
            WalkArea = string.Empty;
            if (element.TryGetProperty("walkArea", out JsonElement walkAreaElement))
            {
                WalkArea = walkAreaElement.GetString() ?? string.Empty;
                ReadOnlyPolygon.GetVertices(WalkArea);
            }

            Placeholders = placeholders.AsReadOnly();

            data.Add(Name, this);
            dataList.Add(this);
        }

        #region Static members

        // All
        public static ReadOnlyCollection<RoomDefinition> All { get; } = new(dataList);

        // Find
        public static RoomDefinition? Find(string name)
        {
            return data.TryGetValue(name, out var definition) ? definition : null;
        }

        // Get
        public static RoomDefinition Get(string name)
        {
            return data[name];
        }

        // Load
        public static void Load(string fileName)
        {
            Utils.LoadJsonData(fileName, element => new RoomDefinition(element));
        }

        #endregion

        // DoorDown
        public Vector2 DoorDown { get; }

        // DoorLeft
        public Vector2 DoorLeft { get; }

        // DoorRight
        public Vector2 DoorRight { get; }

        // DoorUp
        public Vector2 DoorUp { get; }

        // LockType
        public LockType LockType { get; }

        // MaxEnemies
        public int MaxEnemies { get; }

        // MaxProps
        public int MaxProps { get; }

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // RoomType
        public RoomType RoomType { get; }

        // Scope
        public ScopeRules Scope { get; }

        // WalkArea
        public string WalkArea { get; }
    }
}
