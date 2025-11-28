using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// Config
    /// </summary>
    public abstract class Config
    {
        // Constructor
        protected Config(JsonElement element)
        {
            // Name
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            // Weight
            if (element.TryGetProperty("weight", out JsonElement weightElement))
                Weight = weightElement.GetSingle();
            else
                Weight = 1;

            // MaxPerRun
            if (element.TryGetProperty("maxPerRun", out JsonElement maxPerRunElement))
                MaxPerRun = maxPerRunElement.GetInt32();

            // RequiredCompletedRuns
            if (element.TryGetProperty("requiredCompletedRuns", out JsonElement requiredCompletedRunsElement))
                RequiredCompletedRuns = requiredCompletedRunsElement.GetInt32();

            // RequiredRuns
            if (element.TryGetProperty("requiredRuns", out JsonElement requiredRunsElement))
                RequiredRuns = requiredRunsElement.GetInt32();

            // Tags
            Tags = new(ConfigHelper.GetTags(element));

            // Loot
            LootTable = ConfigHelper.GetLoot(element);

            // Pools
            Pools = new(ConfigHelper.GetPools(element));
        }

        // LoadCore
        protected static List<T> LoadCore<T>(string fileName, string rootName, Func<JsonElement, T> onCreate)
        {
            var result = new List<T>();

            using var input = TitleContainer.OpenStream(fileName);
            using JsonDocument doc = JsonDocument.Parse(input);
            var root = doc.RootElement;

            if (!root.TryGetProperty(rootName, out JsonElement arrayElement) || arrayElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException();

            foreach (JsonElement element in arrayElement.EnumerateArray())
            {
                result.Add(onCreate(element));
            }

            return result;
        }

        // LootTable
        public ChanceTable LootTable { get; }

        // MaxPerRun
        public int MaxPerRun { get; }

        // Name
        public string Name { get; }

        // Pools
        public ReadOnlyCollection<string> Pools { get; }

        // RequiredCompletedRuns
        public int RequiredCompletedRuns { get; }

        // RequiredRuns
        public int RequiredRuns { get; }

        // Tags
        public ReadOnlyCollection<string> Tags { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Weight
        public float Weight { get; }
    }
}
