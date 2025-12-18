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

        // FindFurthest
        private static RoomGraph FindFurthest(RoomGraph start)
        {
            Queue<RoomGraph> q = new();
            Dictionary<RoomGraph, int> dists = new();
            q.Enqueue(start); dists.Add(start, 0);
            RoomGraph furthest = start;
            int maxD = 0;

            while (q.Count > 0)
            {
                var curr = q.Dequeue();
                int d = dists[curr];
                if (d > maxD) { maxD = d; furthest = curr; }
                foreach (var n in new[] { curr.Up, curr.Down, curr.Left, curr.Right })
                {
                    if (n != null && n.RoomType != RoomType.Entrance && !dists.ContainsKey(n))
                    {
                        dists.Add(n, d + 1); q.Enqueue(n);
                    }
                }
            }
            return furthest;
        }

        // TryVisit
        private static void TryVisit(RoomGraph? n, int d, Queue<RoomGraph> q, Dictionary<RoomGraph, int> dict)
        {
            if (n != null && n.RoomType != RoomType.Entrance && !dict.ContainsKey(n))
            {
                dict.Add(n, d);
                q.Enqueue(n);
            }
        }

        #endregion

        public static List<RoomGraph> Generate(int seed, int roomCount)
        {
            if (roomCount <= 0) return [];

            Random rng = new(seed);
            Dictionary<(int x, int y), RoomGraph> occupied = new();
            List<RoomGraph> rooms = [];

            // 1. Configuración de entrada (Fija)
            RoomGraph entrance = new(-1, 0, -1, RoomType.Entrance);
            RoomGraph start = new(rooms.Count, 0, 0, RoomType.Start);

            start.Down = entrance;
            entrance.Up = start;

            occupied.Add((0, -1), entrance);
            occupied.Add((0, 0), start);
            rooms.Add(start);

            int attempts = 0;
            // 2. Crecimiento controlado
            while (rooms.Count < roomCount && attempts < 1000)
            {
                attempts++;
                RoomGraph parent = rooms[rng.Next(rooms.Count)];

                // SESGO HORIZONTAL: 0: Up, 1: Down, 2-3: Left, 4-5: Right
                // Esto le da el doble de probabilidad a los lados que a arriba/abajo
                int rawDir = rng.Next(6);
                int dir = rawDir switch
                {
                    0 => 0,
                    1 => 1,
                    2 or 3 => 2,
                    _ => 3
                };

                (int x, int y) target = GetCoords(parent.X, parent.Y, dir);

                if (!occupied.ContainsKey(target))
                {
                    // REGLA DE ORO: Solo se permite si el único vecino es el padre
                    if (CountNeighbors(target, occupied) == 1)
                    {
                        RoomGraph newRoom = new(rooms.Count, target.x, target.y, RoomType.Normal);
                        Link(parent, newRoom, dir);
                        occupied.Add(target, newRoom);
                        rooms.Add(newRoom);
                    }
                }
            }

            // 3. Boss / Habitación especial
            RoomGraph furthest = FindFurthest(start);
            if (furthest != start) furthest.RoomType = RoomType.Coin;

            rooms.Add(entrance);
            return rooms;
        }
    }
}