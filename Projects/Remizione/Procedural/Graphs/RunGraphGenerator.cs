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

        // GetCoords
        private static (int, int) GetCoords(int x, int y, int dir) => dir switch
        {
            0 => (x, y + 1), // Up
            1 => (x, y - 1), // Down
            2 => (x - 1, y), // Left
            3 => (x + 1, y), // Right
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

            q.Enqueue(start);
            dists.Add(start, 0);

            RoomGraph furthest = start;
            int maxD = 0;

            while (q.Count > 0)
            {
                RoomGraph curr = q.Dequeue();
                int d = dists[curr];

                if (d > maxD) { maxD = d; furthest = curr; }

                // Solo navegamos hacia habitaciones que no sean la Entrance para buscar el Boss
                TryVisit(curr.Up, d + 1, q, dists);
                TryVisit(curr.Down, d + 1, q, dists);
                TryVisit(curr.Left, d + 1, q, dists);
                TryVisit(curr.Right, d + 1, q, dists);
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

        // Generate
        public static List<RoomGraph> Generate(int seed, int roomCount)
        {
            if (roomCount <= 0)
                return [];

            Random rng = new(seed);
            Dictionary<(int x, int y), RoomGraph> occupied = [];
            List<RoomGraph> proceduralRooms = [];

            // 1. Configuración de Entrada y Inicio
            RoomGraph entrance = new(-1, 0, -1, RoomType.Entrance);
            RoomGraph start = new(proceduralRooms.Count, 0, 0, RoomType.Start)
            {
                // Forzamos conexión: Start abajo va a Entrance / Entrance arriba va a Start
                Down = entrance
            };
            entrance.Up = start;

            occupied.Add((0, -1), entrance);
            occupied.Add((0, 0), start);
            proceduralRooms.Add(start);

            // 2. Bucle de expansión (estilo Isaac / Árbol)
            while (proceduralRooms.Count < roomCount)
            {
                // Elegimos un padre de los procedurales (nunca de la entrada)
                RoomGraph parent = proceduralRooms[rng.Next(proceduralRooms.Count)];
                int direction = rng.Next(4);
                (int x, int y) coords = GetCoords(parent.X, parent.Y, direction);

                if (!occupied.ContainsKey(coords))
                {
                    RoomGraph newRoom = new(proceduralRooms.Count, coords.x, coords.y, RoomType.Normal);
                    Link(parent, newRoom, direction);

                    occupied.Add(coords, newRoom);
                    proceduralRooms.Add(newRoom);
                }
            }

            // 3. Asignar Boss Room (la más lejana al Start)
            RoomGraph bossRoom = FindFurthest(start);
            if (bossRoom != start)
                bossRoom.RoomType = RoomType.Coin;

            // 4. Consolidar lista final
            // Agregamos la entrada a la lista para devolver el grafo completo
            proceduralRooms.Add(entrance);

            return proceduralRooms;
        }
    }
}