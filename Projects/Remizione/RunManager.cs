using System;
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
        private static RunGraph? runGraph;
        private static readonly Dictionary<string, int> spawnData = [];

        #endregion

        #region Private members

        // CreateRideRoom
        private static RideRoom CreateRideRoom(GameSession session, RoomGraph roomGraph)
        {
            RideRoom? result = null;

            // Default
            if (roomGraph.RoomStyle == RideRoomStyle.Default)
                result = new DefaultRideRoom(session, roomGraph);      

            return result ?? throw new InvalidOperationException("Cannot create ride room.");
        }
        
        #endregion

        // Clear
        public static void Clear()
        {
            foreach (var room in rooms)
            {
                room.Children.Clear();
            }

            runGraph = null;
            entryRooms.Clear();
            rooms.Clear();
            spawnData.Clear();
            HasContent = false;
        }

        // EntryRooms
        public static ReadOnlyCollection<RideRoom> EntryRooms { get; } = entryRooms.AsReadOnly();

        // Generate
        public static void Generate(GameSession session)
        {
            // Create run graph
            runGraph = new RunGraphGenerator(session.Seed).Generate(3);

            // Create procedural rooms
            for (var i = 0; i < runGraph.EntryRooms.Count; i++)
            {
                var isFirstRoom = true;

                foreach (var roomGraph in runGraph.GetRooms(i))
                {
                    var room = CreateRideRoom(session, roomGraph);

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
                if (room.RoomGraph.Id == id)
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
