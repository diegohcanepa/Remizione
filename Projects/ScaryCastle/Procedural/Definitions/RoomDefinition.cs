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
        private readonly List<Placeholder> placeholders = [];
        private readonly List<string> walls = [];

        #region Constructor

        // Constructor
        private RoomDefinition(JsonElement element)
            : base(element)
        {
            DoorDown = element.GetVector2("doorDown");
            DoorLeft = element.GetVector2("doorLeft");
            DoorRight = element.GetVector2("doorRight");
            DoorStyle = element.GetEnum("doorStyle", DoorStyle.Wooden);
            DoorUp = element.GetVector2("doorUp");
            ExactMatch = element.GetBool("exactMatch", false);
            IsMandatory = element.GetBool("isMandatory", false);
            LockType = element.GetEnum("lockType", LockType.None);
            MaxEnemies = element.GetInt32("maxEnemies", -1);
            MaxProps = element.GetInt32("maxProps", -1);
            MusicTag = element.GetString("musicTag");
            
            if (element.GetString("playerPosition") is string playerPositionValue && !string.IsNullOrWhiteSpace(playerPositionValue))
                PlayerPosition = DataConvert.ToVector2(playerPositionValue);
            
            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);
            RoomType = element.GetEnum("roomType", RoomType.SideRoom);
            SideRoomCategory = element.GetEnum("sideRoomCategory", SideRoomCategory.None);
            Theme = element.GetEnum("theme", RoomTheme.BlueStone);

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

                    var allowTags = Tags.FromJson(item, "allowTags");

                    placeholders.Add(new Placeholder(position, placement, fillChance, allowTags, target));
                }
            }

            // Scope
            this.Scope = TagScope.FromJson(element);

            // WalkArea
            WalkArea = string.Empty;
            if (element.TryGetProperty("walkArea", out JsonElement walkAreaElement))
            {
                WalkArea = walkAreaElement.GetString() ?? string.Empty;
                ReadOnlyPolygon.GetVertices(WalkArea);
            }

            // Walls
            if (element.TryGetProperty("walls", out JsonElement wallsElement))
            {
                foreach (var item in wallsElement.EnumerateArray())
                {
                    var value = item.GetString() ?? string.Empty;
                    ReadOnlyPolygon.GetVertices(value);
                    walls.Add(value);
                }
            }

            Validate();

            Placeholders = placeholders.AsReadOnly();
            Walls = walls.AsReadOnly();

            Definitions.Add(this);
        }

        #endregion

        #region Private members

        // Validate
        private void Validate()
        {
            if (RoomType == RoomType.SideRoom && SideRoomCategory == SideRoomCategory.None)
                RaiseValidationError(this, $"Side rooms must have a SideRoomCategory.");

            if (DoorLeft == null && DoorDown == null && DoorRight == null && DoorUp == null)
                RaiseValidationError(this, $"Must have at least one door.");
        }

        #endregion

        // Definitions
        public static DataContainer<RoomDefinition> Definitions { get; } = new(element => new RoomDefinition(element));

        // DoorDown
        public Vector2? DoorDown { get; }

        // DoorLeft
        public Vector2? DoorLeft { get; }

        // DoorRight
        public Vector2? DoorRight { get; }

        // DoorStyle
        public DoorStyle DoorStyle { get; }

        // DoorUp
        public Vector2? DoorUp { get; }

        // ExactMatch
        public bool ExactMatch { get; }

        // HasDownDoor
        public bool HasDownDoor => DoorDown != null;

        // HasLeftDoor
        public bool HasLeftDoor => DoorLeft != null;

        // HasRightDoor
        public bool HasRightDoor => DoorRight != null;

        // HasUpDoor
        public bool HasUpDoor => DoorUp != null;

        // IsMandatory
        public bool IsMandatory { get; }

        // LockType
        public LockType LockType { get; }

        // MaxEnemies
        public int MaxEnemies { get; }

        // MaxProps
        public int MaxProps { get; }

        // MusicTag
        public string MusicTag { get; }

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // PlayerPosition
        public Vector2 PlayerPosition { get; }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // RoomType
        public RoomType RoomType { get; }

        // Scope
        public TagScope Scope { get; }

        // SideRoomCategory
        public SideRoomCategory SideRoomCategory { get; }

        // Theme
        public RoomTheme Theme { get; }

        // WalkArea
        public string WalkArea { get; }

        // Walls
        public ReadOnlyCollection<string> Walls { get; }
    }
}
