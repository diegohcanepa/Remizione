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
    public abstract class ThingConfig : Config
    {
        // Constructor
        protected ThingConfig(JsonElement element)
            : base(element)
        {
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
        }

        // KillGoal
        public int KillGoal { get; }

        // KillGoalReward
        public string KillGoalReward { get; } = string.Empty;

        // MaxPerRoom
        public int MaxPerRoom { get; }
    }
}
