using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle.Procedural
{
    /// <summary>
    /// RunManager
    /// </summary>
    public static class RunManager
    {
        private static readonly List<RoomGraph> rooms = [];

        #region Private members

        // ApplyConfigs
        private static void ApplyConfigs(List<RoomConfig> configList, int maxDistance)
        {
            float threshold = maxDistance / 3f;

            var candidates = new List<RoomConfig>();

            // Rooms
            foreach (var room in rooms)
            {
                // Determinamos la fase según la distancia del room
                var targetDiff = Difficulty.Easy;
                if (room.DistanceFromStart >= threshold * 2)
                    targetDiff = Difficulty.Hard;
                else if (room.DistanceFromStart >= threshold)
                    targetDiff = Difficulty.Normal;

                candidates.Clear();
                foreach (var config in configList)
                {
                    // Match difficulty
                    if ((int)config.Difficulty != (int)targetDiff)
                        continue;

                    if (config.RequiresDeadEnd && !room.IsDeadEnd)
                        continue;

                    if (!config.PassesMaxPerRunConstraint())
                        continue;

                    candidates.Add(config);
                }

                // FALLBACK: Si no hay configs específicas para esa fase, buscamos una inferior
                if (candidates.Count == 0 && targetDiff > Difficulty.Easy)
                {
                    foreach (var config in configList)
                    {
                        if (config.Difficulty < targetDiff)
                        {
                            if (config.RequiresDeadEnd && !room.IsDeadEnd)
                                continue;
                            
                            if (config.PassesMaxPerRunConstraint())
                                candidates.Add(config);
                        }
                    }
                }

                // Pick
                var chanceTable = new ChanceTable();
                foreach (var candidate in candidates)
                {
                    chanceTable.Add(candidate.Name, candidate.Weight, 1, candidate);
                }

                var item = chanceTable.GetValue();
                if (item != null && item.Context is RoomConfig chosenConfig)
                {
                    SpawnCounter.Increment(chosenConfig.Name);
                    room.Config = chosenConfig;
                }
            }
        }

        // GetAvailableConfigs
        private static List<RoomConfig> GetAvailableConfigs(GameSession session, Tags pools)
        {
            var outList = new List<RoomConfig>();

            foreach (var roomConfig in RoomConfig.All)
            {
                // Test unlocked
                if (!session.UnlockedPool.IsUnlocked(roomConfig.Name))
                    continue;

                // Run constraints
                if (!roomConfig.PassesRunConstraints(session))
                    continue;

                // Pools
                if (pools.Count > 0)
                {
                    if (!Utils.Intersects(pools, roomConfig.Pools))
                        continue;
                }

                // Passed all checks
                outList.Add(roomConfig);
            }

            return outList;
        }

        #endregion

        // Clear
        public static void Clear()
        {
            foreach (var room in rooms)
            {
                room.RideRoom?.Children.Clear();
            }

            rooms.Clear();
            SpawnCounter.Reset();
            HasContent = false;
        }

        // Generate
        public static void Generate(GameSession session, Tags pools, int roomCount)
        {
            HasContent = true;
            
            rooms.Clear();
            var result = RunGraphGenerator.Generate(session.Seed, roomCount);
            rooms.AddRange(result.Item1);

            // Get available configs
            var configs = GetAvailableConfigs(session, pools);

            // Assign configs
            ApplyConfigs(configs, result.Item2);

            // Create ride rooms
            foreach (var room in rooms)
            {
                room.RideRoom = RideRoom.CreateInstance(session, room);
            }

            // Load rooms
            foreach (var room in rooms)
            {
                if (room.Config == null)
                    throw new InvalidOperationException($"Room [{room}] has no config.");

                if (room.RideRoom == null)
                    throw new InvalidOperationException($"Room [{room}] has no procedural room.");

                room.RideRoom?.Load();
            }
        }

        // HasContent
        public static bool HasContent { get; private set; }

        // Rooms
        public static ReadOnlyCollection<RoomGraph> Rooms { get; } = rooms.AsReadOnly();

        // SpawnCounter
        public static NamedCounter SpawnCounter { get; } = new();
    }
}
