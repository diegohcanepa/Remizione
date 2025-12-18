using Engendro;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione.Procedural
{
    /// <summary>
    /// RunManager
    /// </summary>
    public static class RunManager
    {
        private static readonly List<RoomGraph> rooms = [];

        #region Private members

        // AssignRoomConfigs
        private static void AssignRoomConfigs(List<RoomConfig> configList)
        {
            var candidates = new List<RoomConfig>();

            // Rooms
            foreach (var room in rooms)
            {
                candidates.Clear();

                foreach (var config in configList)
                {
                    if (!config.PassesMaxPerRunConstraint())
                        continue;

                    candidates.Add(config);
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
        public static void Generate(GameSession session, Tags pools)
        {
            HasContent = true;
            rooms.Clear();
            rooms.AddRange(RunGraphGenerator.Generate(session.Seed, 12));

            // Get available configs
            var availableConfigs = GetAvailableConfigs(session, pools);

            // Assign configs to rooms
            AssignRoomConfigs(availableConfigs);

            // Create ride rooms
            foreach (var room in rooms)
            {
                room.RideRoom = RideRoom.CreateInstance(session, room);
            }

            foreach (var room in rooms)
            {
                room.RideRoom?.Load();
            }
        }

        /*
        // GetRoom
        public static RideRoom? GetRoom(int id)
        {
            foreach (var room in rooms)
            {
                if (room.RoomGraph.Id == id)
                    return room;
            }

            return null;
        }
        */

        // HasContent
        public static bool HasContent { get; private set; }

        // Rooms
        public static ReadOnlyCollection<RoomGraph> Rooms { get; } = rooms.AsReadOnly();

        // SpawnCounter
        public static NamedCounter SpawnCounter { get; } = new();
    }
}
