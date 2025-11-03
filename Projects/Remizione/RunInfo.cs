using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// RunInfo
    /// </summary>
    public static class RunInfo
    {
        private static readonly List<RideRoom> rideRooms = [];
        private static readonly Dictionary<string, int> spawnData = [];

        // Dispose
        public static void Dispose()
        {
            foreach (var room in rideRooms)
            {
                room.Children.Clear();
            }

            rideRooms.Clear();
            spawnData.Clear();

            HasContent = false;
        }

        // Generate
        public static void Generate(GameSession session, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var room = new RideRoom(session, string.Empty, i, i == length - 1);
                rideRooms.Add(room);
            }

            foreach (var room in rideRooms)
            {
                room.Load();
            }

            HasContent = true;
        }

        // GetSpawnCount
        public static int GetSpawnCount(string staticName)
        {
            return spawnData.TryGetValue(staticName, out var value) ? value : 0;
        }

        // HasContent
        public static bool HasContent { get; private set; }

        // LogSpawn
        public static void LogSpawn(string staticName)
        {
            if (spawnData.TryGetValue(staticName, out var value))
                spawnData[staticName] = ++value;
            else
                spawnData[staticName] = 1;
        }

        // RideRooms
        public static ReadOnlyCollection<RideRoom> RideRooms { get; } = rideRooms.AsReadOnly();
    }
}
