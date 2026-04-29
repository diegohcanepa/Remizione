using Engendro;
using Microsoft.Xna.Framework;
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
            AllowEnemies = element.GetBool("allowEnemies", true);
            DoorDown = element.GetVector2("doorDown");
            DoorLeft = element.GetVector2("doorLeft");
            DoorRight = element.GetVector2("doorRight");
            DoorUp = element.GetVector2("doorUp");
            ExactMatch = element.GetBool("exactMatch", false);

            if (element.GetString("exitApproachPosition") is string exitApproachPositionValue && !string.IsNullOrWhiteSpace(exitApproachPositionValue))
                ExitApproachPosition = DataConvert.ToVector2(exitApproachPositionValue);

            ExitHotspot = element.GetString("exitHotspot", string.Empty);
            if (!string.IsNullOrWhiteSpace(ExitHotspot))
                ReadOnlyPolygon.GetVertices(ExitHotspot);

            IsMandatory = element.GetBool("isMandatory", false);
            LockType = element.GetEnum("lockType", LockType.None);
            MusicTag = element.GetString("musicTag");

            if (element.GetString("guardActorPosition") is string guardActorPositionValue && !string.IsNullOrWhiteSpace(guardActorPositionValue))
                GuardActorPosition = DataConvert.ToVector2(guardActorPositionValue);

            if (element.GetString("interactiveActorPosition") is string interactiveActorPositionValue && !string.IsNullOrWhiteSpace(interactiveActorPositionValue))
                InteractiveActorPosition = DataConvert.ToVector2(interactiveActorPositionValue);

            if (element.GetString("leftGatePosition") is string leftGatePositionValue && !string.IsNullOrWhiteSpace(leftGatePositionValue))
                LeftGatePosition = DataConvert.ToVector2(leftGatePositionValue);

            if (element.GetString("leverPosition") is string leverPositionValue && !string.IsNullOrWhiteSpace(leverPositionValue))
                LeverPosition = DataConvert.ToVector2(leverPositionValue);

            if (element.GetString("playerPosition") is string playerPositionValue && !string.IsNullOrWhiteSpace(playerPositionValue))
                PlayerPosition = DataConvert.ToVector2(playerPositionValue);

            if (element.GetString("rightGatePosition") is string rightGatePositionValue && !string.IsNullOrWhiteSpace(rightGatePositionValue))
                RightGatePosition = DataConvert.ToVector2(rightGatePositionValue);

            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);
            RoomType = element.GetEnum("roomType", RoomType.SideRoom);
            SideRoomCategory = element.GetEnum("sideRoomCategory", SideRoomCategory.None);
            Theme = element.GetEnum("theme", RoomTheme.Castle);

            // Placeholders
            if (element.TryGetProperty("placeholders", out JsonElement placeholdersElement))
            {
                foreach (var phElement in placeholdersElement.EnumerateArray())
                {
                    placeholders.Add(new Placeholder(phElement));
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
            if (RoomType == RoomType.Corridor)
            {
                if (LeftGatePosition == Vector2.Zero || RightGatePosition == Vector2.Zero)
                    RaiseValidationError(this, $"Corridors must define gate positions.");

                if (ExitApproachPosition == Vector2.Zero)
                    RaiseValidationError(this, $"Undefined exit approach position.", nameof(ExitApproachPosition));

                if (string.IsNullOrWhiteSpace(ExitHotspot))
                    RaiseValidationError(this, $"Undefined exit hotspot.", nameof(ExitHotspot));
            }

            if (RoomType != RoomType.SideRoom && SideRoomCategory != SideRoomCategory.None)
                RaiseValidationError(this, $"Side room category cannot be specified due to the current room type.");

            if (RoomType == RoomType.SideRoom && SideRoomCategory == SideRoomCategory.None)
                RaiseValidationError(this, $"Side rooms must have a SideRoomCategory.");

            if (DoorLeft == null && DoorDown == null && DoorRight == null && DoorUp == null)
                RaiseValidationError(this, $"Must have at least one door.");
        }

        #endregion

        // AllowEnemies
        public bool AllowEnemies { get; }

        // Definitions
        public static DataContainer<RoomDefinition> Definitions { get; } = new(element => new RoomDefinition(element));

        // DoorDown
        public Vector2? DoorDown { get; }

        // DoorLeft
        public Vector2? DoorLeft { get; }

        // DoorRight
        public Vector2? DoorRight { get; }

        // DoorUp
        public Vector2? DoorUp { get; }

        // ExactMatch
        public bool ExactMatch { get; }

        // ExitApproachPosition
        public Vector2 ExitApproachPosition { get; }

        // ExitHotspot
        public string ExitHotspot { get; }

        // GuardActorPosition
        public Vector2? GuardActorPosition { get; }

        // HasDownDoor
        public bool HasDownDoor => DoorDown != null;

        // HasLeftDoor
        public bool HasLeftDoor => DoorLeft != null;

        // HasRightDoor
        public bool HasRightDoor => DoorRight != null;

        // HasUpDoor
        public bool HasUpDoor => DoorUp != null;

        // InteractiveActorPosition
        public Vector2? InteractiveActorPosition { get; }

        // IsMandatory
        public bool IsMandatory { get; }

        // LeftGatePosition
        public Vector2 LeftGatePosition { get; }

        // LeverPosition
        public Vector2? LeverPosition { get; }

        // LockType
        public LockType LockType { get; }

        // MusicTag
        public string MusicTag { get; }

        // Placeholders
        public ReadOnlyCollection<Placeholder> Placeholders { get; }

        // PlayerPosition
        public Vector2 PlayerPosition { get; }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // RightGatePosition
        public Vector2 RightGatePosition { get; }

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
