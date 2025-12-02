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
    /// ThingConfig
    /// </summary>
    public sealed class ThingConfig : Config
    {
        private static readonly Dictionary<string, ThingConfig> data = [];
        private static readonly List<ThingConfig> dataList = [];

        // Constructor
        public ThingConfig(JsonElement element)
            : base(element)
        {
            // MaxAmount
            if (element.TryGetProperty("maxAmount", out JsonElement maxAmountElement))
                MaxAmount = maxAmountElement.GetInt32();
            else
                MaxAmount = 1;

            // MaxPerRoom
            if (element.TryGetProperty("maxPerRoom", out JsonElement maxPerRoomElement))
                MaxPerRoom = maxPerRoomElement.GetInt32();

            // KillGoal
            if (element.TryGetProperty("killGoal", out JsonElement killGoalElement))
                KillGoal = killGoalElement.GetInt32();

            // KillGoalReward
            if (element.TryGetProperty("killGoalReward", out JsonElement killGoalRewardElement))
            {
                if (killGoalRewardElement.GetString() is string killGoalRewardValue)
                {
                    ConfigHelper.AssertMetaItem(killGoalRewardValue);
                    KillGoalReward = killGoalRewardValue;
                }
            }

            // Unlocked
            if (element.TryGetProperty("unlocked", out JsonElement unlockedElement))
                Unlocked = unlockedElement.GetBoolean();

            data.Add(Name, this);
            dataList.Add(this);
        }

        #region Static members

        // All
        public static ReadOnlyCollection<ThingConfig> All { get; } = new(dataList);

        // Find
        public static ThingConfig? Find(string name)
        {
            return data.TryGetValue(name, out ThingConfig? config) ? config : null;
        }

        // Load
        public static void Load(params string[] fileNames)
        {
            for (var i = 0; i < fileNames.Length; i++)
            {
                Utils.LoadJsonData<ThingConfig>(fileNames[i], (JsonElement element) => new ThingConfig(element));
            }
        }

        #endregion

        // KillGoal
        public int KillGoal { get; }

        // KillGoalReward
        public string KillGoalReward { get; } = string.Empty;

        // MaxAmount
        public int MaxAmount { get; }

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // PassesMaxPerRoomConstraint
        public bool PassesMaxPerRoomConstraint(int instanceCount)
        {
            if (MaxPerRoom > 0 && instanceCount >= MaxPerRoom)
                return false;
            else
                return true;
        }

        // Unlocked
        public bool Unlocked { get; }
    }
}
