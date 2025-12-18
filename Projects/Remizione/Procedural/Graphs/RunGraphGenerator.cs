using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RunGraphGenerator
    /// </summary>
    public static class RunGraphGenerator
    {
        #region Private members

        // CalculateDistances
        private static RoomGraph CalculateDistances(RoomGraph start)
        {
            Queue<RoomGraph> queue = new();
            Dictionary<RoomGraph, int> visited = [];

            queue.Enqueue(start);
            visited.Add(start, 0);
            start.DistanceFromStart = 0;

            RoomGraph furthest = start;
            int maxDist = 0;

            while (queue.Count > 0)
            {
                RoomGraph current = queue.Dequeue();
                int d = visited[current];

                if (d > maxDist)
                {
                    maxDist = d;
                    furthest = current;
                }

                // Revisar los 4 vecinos conectados
                Span<RoomGraph?> neighbors = [current.Up, current.Down, current.Left, current.Right];
                foreach (var n in neighbors)
                {
                    // No contamos la entrada como parte del reto de distancia
                    if (n != null && n.RoomType != RoomType.Entrance && !visited.ContainsKey(n))
                    {
                        n.DistanceFromStart = d + 1;
                        visited.Add(n, d + 1);
                        queue.Enqueue(n);
                    }
                }
            }
            return furthest;
        }

        // CountNeighbors
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
        private static (int, int) GetCoords(int x, int y, int dir) => dir switch
        {
            0 => (x, y + 1),
            1 => (x, y - 1),
            2 => (x - 1, y),
            3 => (x + 1, y),
            _ => (x, y)
        };

        // Link
        private static void Link(RoomGraph p, RoomGraph c, int dir)
        {
            switch (dir)
            {
                case 0: p.Up = c; c.Down = p; break;
                case 1: p.Down = c; c.Up = p; break;
                case 2: p.Left = c; c.Right = p; break;
                case 3: p.Right = c; c.Left = p; break;
            }
        }

        #endregion

        public static List<RoomGraph> Generate(int seed, int roomCount)
        {
            if (roomCount <= 0)
                return [];

            Random rng = new(seed);
            Dictionary<(int x, int y), RoomGraph> occupied = [];
            List<RoomGraph> rooms = [];

            // 1. Setup
            RoomGraph entrance = new(-1, 0, -1, RoomType.Entrance);
            RoomGraph start = new(rooms.Count, 0, 0, RoomType.Start);
            start.Down = entrance;
            entrance.Up = start;

            occupied.Add((0, -1), entrance);
            occupied.Add((0, 0), start);
            rooms.Add(start);

            // 2. Generation with Unique Neighbor Rule and Horizontal Bias
            int attempts = 0;
            while (rooms.Count < roomCount && attempts < 1000)
            {
                attempts++;
                RoomGraph parent = rooms[rng.Next(rooms.Count)];

                // Sesgo horizontal (más probabilidad a izquierda/derecha)
                int rawDir = rng.Next(6);
                int dir = rawDir switch
                {
                    0 => 0,
                    1 => 1,
                    2 or 3 => 2,
                    _ => 3
                };

                (int x, int y) target = GetCoords(parent.X, parent.Y, dir);

                if (!occupied.ContainsKey(target) && CountNeighbors(target, occupied) == 1)
                {
                    RoomGraph newRoom = new(rooms.Count, target.x, target.y, RoomType.Normal);
                    Link(parent, newRoom, dir);
                    occupied.Add(target, newRoom);
                    rooms.Add(newRoom);
                }
            }

            // 3. Calcular Distancias y encontrar la Coin Room
            // Usamos BFS para setear 'DistanceFromStart' en cada room
            RoomGraph coinRoom = CalculateDistances(start);
            if (coinRoom != start)
                coinRoom.RoomType = RoomType.Coin;

            // La entrada queda fuera del árbol procedural, le ponemos distancia negativa o 0
            entrance.DistanceFromStart = -1;

            rooms.Add(entrance);
            return rooms;
        }
    }
}