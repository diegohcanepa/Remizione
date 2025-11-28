using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// PropConfig
    /// name
    /// loot
    /// maxPerRoom
    /// pools
    /// tags
    /// weight
    /// </summary>
    public sealed class PropConfig : Config
    {
        private static readonly Dictionary<string, PropConfig> data = [];

        // Constructor
        private PropConfig(string name, IList<string> tags, ChanceTable lootTable, IList<string> pools, float weight, int maxPerRoom, int maxPerRun, int requiredRuns, int requiredCompletedRuns)
            : base(name, tags, lootTable)
        {
            this.Pools = new(pools);
            this.Weight = weight;
            this.MaxPerRoom = maxPerRoom;
            this.MaxPerRun = maxPerRun;
            this.RequiredRuns = requiredRuns;
            this.RequiredCompletedRuns = requiredCompletedRuns;
        }

        #region Static members

        // GetConfig
        public static PropConfig? GetConfig(string propName)
        {
            return data.TryGetValue(propName, out PropConfig? config) ? config : null;
        }

        // Load
        public static void Load(string fileName)
        {
            try
            {
                using var input = TitleContainer.OpenStream(fileName);
                using JsonDocument doc = JsonDocument.Parse(input);
                var root = doc.RootElement;

                if (!root.TryGetProperty("props", out JsonElement propsArray) || propsArray.ValueKind != JsonValueKind.Array)
                    throw new InvalidDataException("Prop is not an array.");

                foreach (JsonElement propElement in propsArray.EnumerateArray())
                {
                    // Name
                    if (propElement.GetProperty("name").GetString() is not string propName)
                        throw new InvalidDataException("Prop name not found.");

                    // Weight
                    float weight = 1;
                    if (propElement.TryGetProperty("weight", out JsonElement weightElement))
                        weight = weightElement.GetSingle();

                    // MaxPerRoom
                    int maxPerRoom = -1;
                    if (propElement.TryGetProperty("maxPerRoom", out JsonElement maxPerRoomElement))
                        maxPerRoom = maxPerRoomElement.GetInt32();

                    // MaxPerRun
                    var maxPerRun = 0;
                    if (propElement.TryGetProperty("maxPerRun", out JsonElement maxPerRunElement))
                        maxPerRun = maxPerRunElement.GetInt32();

                    // RequiredCompletedRuns
                    var requiredCompletedRuns = 0;
                    if (propElement.TryGetProperty("requiredCompletedRuns", out JsonElement requiredCompletedRunsElement))
                        requiredCompletedRuns = requiredCompletedRunsElement.GetInt32();

                    // RequiredRuns
                    var requiredRuns = 0;
                    if (propElement.TryGetProperty("requiredRuns", out JsonElement requiredRunsElement))
                        requiredRuns = requiredRunsElement.GetInt32();

                    // Tags
                    var tags = ConfigHelper.GetTags(propElement);

                    // Loot
                    var loot = ConfigHelper.GetLoot(propElement);

                    // Pools
                    var pools = ConfigHelper.GetPools(propElement);

                    // Add configuration
                    var propConfig = new PropConfig(propName, tags, loot, pools, weight, maxPerRoom, maxPerRun, requiredRuns, requiredCompletedRuns);
                    data.Add(propName, propConfig);
                }
            }
            catch (FileNotFoundException)
            {
            }
        }

        #endregion

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // MaxPerRun
        public int MaxPerRun { get; }

        // Pools
        public ReadOnlyCollection<string> Pools { get; }

        // RequiredCompletedRuns
        public int RequiredCompletedRuns { get; }

        // RequiredRuns
        public int RequiredRuns { get; }

        // ToString
        public override string ToString() => Name;

        // Weight
        public float Weight { get; }
    }
}
