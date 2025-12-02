using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

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
            SpawnCounter.Reset();
            HasContent = false;
        }

        // EntryRooms
        public static ReadOnlyCollection<RideRoom> EntryRooms { get; } = entryRooms.AsReadOnly();

        // Generate
        public static void Generate(GameSession session)
        {
            HasContent = true;

            // Create run graph
            runGraph = new RunGraphGenerator(session.Seed).Generate(session, 3);

            // Create procedural rooms
            for (var i = 0; i < runGraph.EntryRooms.Count; i++)
            {
                var isFirstRoom = true;

                foreach (var roomGraph in runGraph.GetRooms(i))
                {
                    var room = RideRoom.CreateInstance(session, roomGraph);

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

        // HasContent
        public static bool HasContent { get; private set; }

        // Rooms
        public static ReadOnlyCollection<RideRoom> Rooms { get; } = rooms.AsReadOnly();

        // SpawnCounter
        public static NamedCounter SpawnCounter { get; } = new();
    }
}
