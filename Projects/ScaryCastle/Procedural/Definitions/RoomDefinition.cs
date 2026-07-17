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
        private readonly List<LightDescriptor> lights = [];
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
                Polygon.GetVertices(ExitHotspot);

            IsMandatory = element.GetBool("isMandatory", false);

            LightMapColor = element.GetColor("lightMapColor", new Color(20, 20, 20));

            LockType = element.GetEnum("lockType", LockType.None);
            MusicTag = element.GetString("musicTag");

            if (element.GetString("bossPosition") is string bossPositionValue && !string.IsNullOrWhiteSpace(bossPositionValue))
                BossPosition = DataConvert.ToVector2(bossPositionValue);

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

            if (element.GetEnum<RoomCategory>("roomCategory") is not RoomCategory roomCategory)
                throw new InvalidOperationException("Missing roomCategory property.");
            else
                this.RoomCategory = roomCategory;

            Theme = element.GetEnum("theme", RoomTheme.Castle);

            // Lights
            if (element.TryGetProperty("lights", out JsonElement lightsElement))
            {
                foreach (var lightElement in lightsElement.EnumerateArray())
                {
                    lights.Add(new(lightElement));
                }
            }

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
                Polygon.GetVertices(WalkArea);
            }

            // Walls
            if (element.TryGetProperty("walls", out JsonElement wallsElement))
            {
                foreach (var item in wallsElement.EnumerateArray())
                {
                    var value = item.GetString() ?? string.Empty;
                    Polygon.GetVertices(value);
                    walls.Add(value);
                }
            }

            Validate();

            Lights = lights.AsReadOnly();
            Placeholders = placeholders.AsReadOnly();
            Walls = walls.AsReadOnly();

            Definitions.Add(this);
        }

        #endregion

        #region Private members

        // Validate
        private void Validate()
        {
            if (DoorLeft == null && DoorDown == null && DoorRight == null && DoorUp == null)
                RaiseValidationError(this, $"Must have at least one door.");
        }

        #endregion

        // AllowEnemies
        public bool AllowEnemies { get; }

        // BossPosition
        public Vector2? BossPosition { get; }

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

        // LightMapColor
        public Color LightMapColor { get; }

        // Lights
        public ReadOnlyCollection<LightDescriptor> Lights { get; }

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

        // RoomCategory
        public RoomCategory RoomCategory { get; }

        // Scope
        public TagScope Scope { get; }

        // Theme
        public RoomTheme Theme { get; }

        // WalkArea
        public string WalkArea { get; }

        // Walls
        public ReadOnlyCollection<string> Walls { get; }
    }
}
