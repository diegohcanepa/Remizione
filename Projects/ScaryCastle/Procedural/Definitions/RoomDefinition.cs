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
            DoorDown = element.GetVector2("doorDown", Vector2.Zero);
            DoorLeft = element.GetVector2("doorLeft", Vector2.Zero);
            DoorRight = element.GetVector2("doorRight", Vector2.Zero);
            DoorUp = element.GetVector2("doorUp", Vector2.Zero);
            IsStartingRoom = element.GetBool("isStartingRoom", false);
            LockType = element.GetEnum("lockType", LockType.None);
            MaxEnemies = element.GetInt32("maxEnemies", -1);
            MaxProps = element.GetInt32("maxProps", -1);
            MusicTag = element.GetString("musicTag");
            RequiresDeadEnd = element.GetBool("requiresDeadEnd", false);

            if (DoorLeft == Vector2.Zero || DoorDown == Vector2.Zero || DoorRight == Vector2.Zero || DoorUp == Vector2.Zero)
                throw new InvalidOperationException($"Missing doors [{Name}].");

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

            Placeholders = placeholders.AsReadOnly();
            Walls = walls.AsReadOnly();

            Definitions.Add(this);
        }

        #endregion

        // Definitions
        public static DataContainer<RoomDefinition> Definitions { get; } = new(element => new RoomDefinition(element));

        // DoorDown
        public Vector2 DoorDown { get; }

        // DoorLeft
        public Vector2 DoorLeft { get; }

        // DoorRight
        public Vector2 DoorRight { get; }

        // DoorUp
        public Vector2 DoorUp { get; }

        // IsStartingRoom
        public bool IsStartingRoom { get; }

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

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // Scope
        public ScopeRules Scope { get; }

        // Validate
        public override void Validate(GameSession session)
        {
            base.Validate(session);

            if (IsStartingRoom)
            {
                // Rule: Difficulty must be easy
                if (Difficulty != Difficulty.Easy)
                    RaiseValidationError(this, "A starting room must have easy difficulty.", nameof(Difficulty));

                // Rule: MaxEnemies not allowed
                if (MaxEnemies > 0)
                    RaiseValidationError(this, "A starting room cannot define maximum enemies.", nameof(MaxEnemies));

                // Rule: Dead end not allowed
                if (RequiresDeadEnd)
                    RaiseValidationError(this, "A starting room cannot be a dead end.", nameof(RequiresDeadEnd));
            }
        }

        // WalkArea
        public string WalkArea { get; }

        // Walls
        public ReadOnlyCollection<string> Walls { get; }
    }
}
