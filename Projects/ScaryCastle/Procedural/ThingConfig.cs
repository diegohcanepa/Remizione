using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ThingConfig
    /// </summary>
    public sealed class ThingConfig : Config
    {
        private static readonly Dictionary<string, ThingConfig> data = [];
        private static readonly List<ThingConfig> dataList = [];

        #region Constructor

        // Constructor
        public ThingConfig(JsonElement element)
            : base(element)
        {
            // InteractionGoal
            if (element.TryGetProperty("interactionGoal", out JsonElement interactionGoalElement))
                InteractionGoal = interactionGoalElement.GetInt32();

            // InteractionGoalReward
            if (element.TryGetProperty("interactionGoalReward", out JsonElement interactionGoalRewardElement))
            {
                if (interactionGoalRewardElement.GetString() is string interactionGoalRewardValue)
                    InteractionGoalReward = new(interactionGoalRewardValue.Split(','));
            }

            // KillGoal
            if (element.TryGetProperty("killGoal", out JsonElement killGoalElement))
                KillGoal = killGoalElement.GetInt32();

            // KillGoalReward
            if (element.TryGetProperty("killGoalReward", out JsonElement killGoalRewardElement))
            {
                if (killGoalRewardElement.GetString() is string killGoalRewardValue)
                    KillGoalReward = new(killGoalRewardValue.Split(','));
            }

            // MaxPerRoom
            MaxPerRoom = -1;
            if (element.TryGetProperty("maxPerRoom", out JsonElement maxPerRoomElement))
                MaxPerRoom = Math.Max(MaxPerRoom, maxPerRoomElement.GetInt32());

            // MaxSpawnAmount
            MaxSpawnAmount = 1;
            if (element.TryGetProperty("maxSpawnAmount", out JsonElement maxSpawnAmountElement))
                MaxSpawnAmount = Math.Max(1, maxSpawnAmountElement.GetInt32());

            // MinSpawnAmount
            MinSpawnAmount = 1;
            if (element.TryGetProperty("minSpawnAmount", out JsonElement minSpawnAmountElement))
                MinSpawnAmount = Math.Max(1, minSpawnAmountElement.GetInt32());

            if (MinSpawnAmount > MaxSpawnAmount)
                throw new InvalidOperationException($"[{Name}]: {nameof(MinSpawnAmount)} cannot be greater than MaxSpawnAmount.");

            // RequiresDeadEnd
            if (element.TryGetProperty("requiresDeadEnd", out JsonElement requiresDeadEndElement))
                RequiresDeadEnd = requiresDeadEndElement.GetBoolean();

            // UsePlaceholder
            if (element.TryGetProperty("usePlaceholder", out JsonElement usePlaceholderElement))
                UsePlaceholder = usePlaceholderElement.GetBoolean();

            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

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
                Utils.LoadJsonData<ThingConfig>(fileNames[i], element => new ThingConfig(element));
            }
        }

        #endregion

        // InteractionGoal
        public int InteractionGoal { get; }

        // InteractionGoalReward
        public ReadOnlyCollection<string> InteractionGoalReward { get; } = [];

        // KillGoal
        public int KillGoal { get; }

        // KillGoalReward
        public ReadOnlyCollection<string> KillGoalReward { get; } = [];

        // MaxPerRoom
        public int MaxPerRoom { get; }

        // MaxSpawnAmount
        public int MaxSpawnAmount { get; }

        // MinSpawnAmount
        public int MinSpawnAmount { get; }

        // PassesMaxPerRoomConstraint
        public bool PassesMaxPerRoomConstraint(int instanceCount)
        {
            return MaxPerRoom == -1 || instanceCount < MaxPerRoom;
        }

        // RequiresDeadEnd
        public bool RequiresDeadEnd { get; }

        // UsePlaceholder
        public bool UsePlaceholder { get; }

        // Validate
        public override void Validate(GameSession session)
        {
            base.Validate(session);

            if (session.FindDeclaredThing(Name) == null)
                throw new InvalidOperationException($"[{Name}] has no script declaration.");

            ValidateNameReferences(nameof(InteractionGoalReward), InteractionGoalReward);
            ValidateNameReferences(nameof(InteractionGoalReward), KillGoalReward);
        }
    }
}