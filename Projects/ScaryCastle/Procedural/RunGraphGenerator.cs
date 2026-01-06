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
        private static (int, int) GetCoords(int x, int y, int dir)
        {
            return dir switch
            {
                0 => (x, y + 1),
                1 => (x, y - 1),
                2 => (x - 1, y),
                3 => (x + 1, y),
                _ => (x, y)
            };
        }

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

        // ProcessMapData
        private static RoomGraph ProcessMapData(RoomGraph start)
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

                // Revisar conexiones existentes
                RoomGraph?[] neighbors = [current.Up, current.Down, current.Left, current.Right];
                foreach (var n in neighbors)
                {
                    if (n != null && !visited.ContainsKey(n))
                    {
                        n.DistanceFromStart = d + 1;
                        visited.Add(n, d + 1);
                        queue.Enqueue(n);
                    }
                }
            }
            return furthest;
        }

        #endregion

        // Generate
        public static (List<RoomGraph>, int) Generate(Random rng, int roomCount)
        {
            if (roomCount <= 0)
                return ([], 0);

            Dictionary<(int x, int y), RoomGraph> occupied = [];
            List<RoomGraph> rooms = [];

            // 1. Setup Start
            RoomGraph start = new(rooms.Count, 0, 0, RoomType.Start);
            occupied.Add((0, 0), start);
            rooms.Add(start);

            // Bloqueamos la coordenada de abajo para que nada se genere ahí
            // y el Start.Down permanezca siempre null.
            occupied.Add((0, -1), null!);

            // 2. Bucle de Generación
            int attempts = 0;
            while (rooms.Count < roomCount && attempts < 2000)
            {
                attempts++;
                RoomGraph parent = rooms[rng.Next(rooms.Count)];

                // Sesgo horizontal para mejor visibilidad (Izquierda/Derecha son más probables)
                int dir = rng.Next(6) switch
                {
                    0 => 0,
                    1 => 1,
                    2 or 3 => 2,
                    _ => 3
                };

                // Evitar que el Start intente conectar hacia abajo
                if (parent.RoomType == RoomType.Start && dir == 1) continue;

                (int x, int y) target = GetCoords(parent.X, parent.Y, dir);

                // Regla de Isaac: No se puede tocar con habitaciones que no sean el padre
                if (!occupied.ContainsKey(target) && CountNeighbors(target, occupied) == 1)
                {
                    RoomGraph newRoom = new(rooms.Count, target.x, target.y, RoomType.Normal);
                    Link(parent, newRoom, dir);
                    occupied.Add(target, newRoom);
                    rooms.Add(newRoom);
                }
            }

            // 3. Procesar Distancias y marcar la Moneda
            RoomGraph coinRoom = ProcessMapData(start);
            if (coinRoom != start)
            {
                coinRoom.RoomType = RoomType.Coin;
                coinRoom.HasCoin = true;
            }

            // Devolvemos el maxDist (la distancia a la moneda) para los cálculos de fases
            int maxDist = coinRoom.DistanceFromStart;
            return (rooms, maxDist);
        }
    }
}