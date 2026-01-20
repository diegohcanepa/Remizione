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

        #region Constructor

        // Constructor
        private RoomDefinition(JsonElement element)
            : base(element)
        {
            DoorDown = element.GetVector2("doorDown");
            DoorLeft = element.GetVector2("doorLeft");
            DoorRight = element.GetVector2("doorRight");
            DoorUp = element.GetVector2("doorUp");
            LockType = element.GetEnum<LockType>("lockType", LockType.None);
            MaxEnemies = element.GetInt32("maxEnemies", -1);
            MaxProps = element.GetInt32("maxProps", -1);
            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);
            RoomType = element.GetEnum<RoomType>("roomType", RoomType.Connector);

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

        #endregion

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
