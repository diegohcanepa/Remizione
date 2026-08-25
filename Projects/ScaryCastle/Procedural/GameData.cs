using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// GameData
    /// </summary>
    internal static class GameData
    {
        #region Private members

        // Load
        private static DataContainer<T> Load<T>(string fileName, Func<JsonElement, T> onCreate)
            where T : INamedObject
        {
            fileName = Path.Combine("Content", ContentFolder.System.ToString(), fileName);

            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out JsonElement arrayElement) || arrayElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException();

            var dict = new Dictionary<string, T>();

            foreach (JsonElement element in arrayElement.EnumerateArray())
            {
                var obj = onCreate(element);
                dict.Add(obj.Name, obj);
            }

            return new DataContainer<T>(dict.ToFrozenDictionary());
        }

        #endregion

        // Actors
        public static DataContainer<ActorDefinition> Actors { get; private set; } = null!;

        // CombatBehaviors
        public static DataContainer<CombatBehavior> CombatBehaviors { get; private set; } = null!;

        // IsLoaded
        public static bool IsLoaded { get; private set; }

        // Items
        public static DataContainer<ItemDefinition> Items { get; private set; } = null!;

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
            Rooms = Load("Rooms.json", e => new RoomDefinition(e));
            Actors = Load("Actors.json", e => new ActorDefinition(e));
            Props = Load("Props.json", e => new PropDefinition(e));
        }

        // Props
        public static DataContainer<PropDefinition> Props { get; private set; } = null!;

        // Rooms
        public static DataContainer<RoomDefinition> Rooms { get; private set; } = null!;

        // RunModifiers
        public static DataContainer<RunModifierDefinition> RunModifiers { get; private set; } = null!;

        // Runs
        public static DataContainer<RunDefinition> Runs { get; private set; } = null!;

        // Statuses
        public static DataContainer<StatusDefinition> Statuses { get; private set; } = null!;

        // Traits
        public static DataContainer<TraitDefinition> Traits { get; private set; } = null!;
    }
}
 