using Engendro;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// GameData
    /// </summary>
    public static class GameData
    {
        #region Private members

        // Load
        private static FrozenNamedCollection<T> Load<T>(string fileName, Func<JsonElement, T> onCreate)
            where T : class, INamedObject
        {
            fileName = Path.Combine("Content", ContentFolder.System.ToString(), fileName);

            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out JsonElement arrayElement) || arrayElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException();

            var list = new List<T>();

            foreach (JsonElement element in arrayElement.EnumerateArray())
            {
                var obj = onCreate(element);
                list.Add(obj);
            }

            return new FrozenNamedCollection<T>(list);
        }

        #endregion

        // Actors
        public static FrozenNamedCollection<ActorDefinition> Actors { get; private set; } = null!;

        // CombatBehaviors
        public static FrozenNamedCollection<CombatBehavior> CombatBehaviors { get; private set; } = null!;

        // IsLoaded
        public static bool IsLoaded { get; private set; }

        // Items
        public static FrozenNamedCollection<ItemDefinition> Items { get; private set; } = null!;

        // Load
        public static void Load()
        {
            if (IsLoaded)
                throw new InvalidOperationException("Data already loaded.");

            Runs = Load("Runs.json", e => new RunDefinition(e));
            RunModifiers = Load("RunModifiers.json", e => new RunModifierDefinition(e));
            CombatBehaviors = Load("CombatBehaviors.json", e => new CombatBehavior(e));
            Statuses = Load("Statuses.json", e => new StatusDefinition(e));
            Traits = Load("Traits.json", e => new TraitDefinition(e));

            Items = Load("Items.json", e => new ItemDefinition(e));
            Actors = Load("Actors.json", e => new ActorDefinition(e));
            Props = Load("Props.json", e => new PropDefinition(e));
            Rooms = Load("Rooms.json", e => new RoomDefinition(e));
        }

        // Props
        public static FrozenNamedCollection<PropDefinition> Props { get; private set; } = null!;

        // Rooms
        public static FrozenNamedCollection<RoomDefinition> Rooms { get; private set; } = null!;

        // RunModifiers
        public static FrozenNamedCollection<RunModifierDefinition> RunModifiers { get; private set; } = null!;

        // Runs
        public static FrozenNamedCollection<RunDefinition> Runs { get; private set; } = null!;

        // Statuses
        public static FrozenNamedCollection<StatusDefinition> Statuses { get; private set; } = null!;

        // Traits
        public static FrozenNamedCollection<TraitDefinition> Traits { get; private set; } = null!;
    }
}
