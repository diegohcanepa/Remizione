using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// RunGraphGenerator
    /// </summary>
    public static class RunGraphGenerator
    {
        #region Private members

        // CountNeighbors
        // Counts existing rooms adjacent to a specific coordinate.
        private static int CountNeighbors((int x, int y) c, Dictionary<(int x, int y), RoomGraph> map)
        {
            int count = 0;
            if (map.ContainsKey((c.x, c.y + 1)))
                count++;

            if (map.ContainsKey((c.x, c.y - 1)))
                count++;

            if (map.ContainsKey((c.x - 1, c.y)))
                count++;

            if (map.ContainsKey((c.x + 1, c.y)))
                count++;

            return count;
        }

        // GetCoords
        // Returns the grid coordinates for a neighbor in a given direction.
        private static (int x, int y) GetCoords(int x, int y, int direction)
        {
            return direction switch
            {
                0 => (x, y + 1), // Up
                1 => (x, y - 1), // Down
                2 => (x - 1, y), // Left
                3 => (x + 1, y), // Right
                _ => (x, y)
            };
        }

        // Link
        // Bi-directionally connects two room nodes.
        private static void Link(RoomGraph p, RoomGraph c, int direction)
        {
            switch (direction)
            {
                case 0:
                    p.Up = c; c.Down = p;
                    break;

                case 1:
                    p.Down = c; c.Up = p;
                    break;

                case 2:
                    p.Left = c; c.Right = p;
                    break;

                case 3:
                    p.Right = c; c.Left = p;
                    break;
            }
        }

        // ProcessMapData
        // Selecciona la salida analizando la disponibilidad de espacio en la grilla sin alterar el orden original.
        private static RoomGraph ProcessMapData(RoomGraph start, List<RoomGraph> rooms, Dictionary<(int x, int y), RoomGraph> map)
        {
            for (var i = 0; i < rooms.Count; i++)
            {
                rooms[i].Visited = false;
                rooms[i].DistanceFromStart = 0;
            }

            var queue = new Queue<RoomGraph>();
            start.Visited = true;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                void CheckNeighbor(RoomGraph? neighbor)
                {
                    if (neighbor == null || neighbor.Visited)
                        return;

                    neighbor.Visited = true;
                    neighbor.DistanceFromStart = current.DistanceFromStart + 1;
                    queue.Enqueue(neighbor);
                }

                CheckNeighbor(current.Up);
                CheckNeighbor(current.Down);
                CheckNeighbor(current.Left);
                CheckNeighbor(current.Right);
            }

            // CORRECCIÓN: Creamos una lista temporal para buscar la salida sin romper el índice 0 del motor.
            var searchList = new List<RoomGraph>(rooms);
            searchList.Sort((a, b) => b.DistanceFromStart.CompareTo(a.DistanceFromStart));

            foreach (var room in searchList)
            {
                if (room == start)
                    continue;

                if (!map.ContainsKey((room.X + 1, room.Y)))
                {
                    room.RoomType = RoomType.RightExit;
                    return room;
                }

                if (!map.ContainsKey((room.X - 1, room.Y)))
                {
                    room.RoomType = RoomType.LeftExit;
                    return room;
                }
            }

            return start;
        }

        #endregion

        // Generate
        // Generates the room layout topology.
        public static (List<RoomGraph>, int) Generate(Random rng, int roomCount)
        {
            List<RoomGraph> rooms = [];
            Dictionary<(int x, int y), RoomGraph> occupied = [];

            RoomGraph start = new RoomGraph(0, 0, 0, RoomType.Start);
            occupied.Add((0, 0), start);
            rooms.Add(start);

            int maxAttempts = 3000;
            int currentAttempt = 0;

            while (rooms.Count < roomCount && currentAttempt < maxAttempts)
            {
                currentAttempt++;
                RoomGraph parent = rooms[rng.Next(rooms.Count)];
                int direction = rng.Next(4);

                if (parent.RoomType == RoomType.Start && direction == 1)
                    continue;

                (int x, int y) target = GetCoords(parent.X, parent.Y, direction);
                if (occupied.ContainsKey(target))
                    continue;

                if (CountNeighbors(target, occupied) >= 2)
                    continue;

                RoomGraph newRoom = new RoomGraph(rooms.Count, target.x, target.y, RoomType.Connector);
                Link(parent, newRoom, direction);
                occupied.Add(target, newRoom);
                rooms.Add(newRoom);
            }

            RoomGraph exitRoom = ProcessMapData(start, rooms, occupied);

            if (exitRoom == start)
                return (rooms, -1);

            return (rooms, exitRoom.Index);
        }
    }
}