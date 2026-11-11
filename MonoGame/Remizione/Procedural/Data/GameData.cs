using Engendro;
using Engendro.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Remizione
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

        // TextRepository_Loaded
        private static void TextRepository_Loaded()
        {
            foreach (var item in Items)
            {
                item.RefreshLocalizedValues();
            }
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

            WorldSettings = new();
            CombatBehaviors = Load("CombatBehaviors.json", e => new CombatBehavior(e));
            Rooms = Load("Rooms.json", e => new RoomDefinition(e));
            Items = Load("Items.json", e => new ItemDefinition(e));
            Actors = Load("Actors.json", e => new ActorDefinition(e));
            Props = Load("Props.json", e => new PropDefinition(e));

            TextRepository.Loaded += TextRepository_Loaded;

            IsLoaded = true;
        }

        // Props
        public static FrozenNamedCollection<PropDefinition> Props { get; private set; } = null!;

        // Rooms
        public static FrozenNamedCollection<RoomDefinition> Rooms { get; private set; } = null!;

        // WorldSettings
        public static WorldSettings WorldSettings { get; private set; } = null!;
    }
}
