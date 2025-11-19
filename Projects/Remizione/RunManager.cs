using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// RunManager
    /// </summary>
    public static class RunManager
    {
        #region Private fields

        private static readonly List<RideRoom> entryRooms = [];
        private static readonly List<RideRoom> rooms = [];
        private static RunDescriptor? runDescriptor;
        private static readonly Dictionary<string, int> spawnData = [];

        #endregion

        #region Private members

        // CreateRideRoom
        private static RideRoom CreateRideRoom(GameSession session, RoomDescriptor descriptor)
        {
            return new RideRoom(session, descriptor);
        }
        
        #endregion

        // Clear
        public static void Clear()
        {
            foreach (var room in rooms)
            {
                room.Children.Clear();
            }

            entryRooms.Clear();
            rooms.Clear();
            spawnData.Clear();
            HasContent = false;
        }

        // EntryRooms
        public static ReadOnlyCollection<RideRoom> EntryRooms { get; } = entryRooms.AsReadOnly();

        // Generate
        public static void Generate(GameSession session, int length)
        {
            // Create run descriptor
            runDescriptor = new RunDescriptorGenerator(session.Seed).Generate(3);

            // Create all rooms from all paths
            for (var i = 0; i < runDescriptor.Paths.Length; i++)
            {
                var isFirstRoom = true;

                foreach (var roomDescriptor in runDescriptor.GetRoomDescriptors(i))
                {
                    var room = CreateRideRoom(session, roomDescriptor);

                    if (isFirstRoom)
                    {
                        entryRooms.Add(room);
                        isFirstRoom = false;
                    }

                    rooms.Add(room);
                }
            }

            foreach (var room in rooms)
            {
                room.Load();
            }

            HasContent = true;
        }

        // GetRoom
        public static RideRoom? GetRoom(int id)
        {
            foreach (var room in rooms)
            {
                if (room.Descriptor.Id == id)
                    return room;    
            }

            return null;
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

        // Rooms
        public static ReadOnlyCollection<RideRoom> Rooms { get; } = rooms.AsReadOnly();
    }
}
