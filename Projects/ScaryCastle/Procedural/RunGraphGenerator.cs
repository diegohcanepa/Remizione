using System;
using System.Collections.Generic;

namespace ScaryCastle
{
    public static class RunGraphGenerator
    {
        #region Private members

        private static int CountNeighbors((int x, int y) c, Dictionary<(int x, int y), RoomGraph> map)
        {
            int count = 0;
            if (map.ContainsKey((c.x, c.y + 1))) count++;
            if (map.ContainsKey((c.x, c.y - 1))) count++;
            if (map.ContainsKey((c.x - 1, c.y))) count++;
            if (map.ContainsKey((c.x + 1, c.y))) count++;
            return count;
        }

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

        private static void Link(RoomGraph p, RoomGraph c, int direction)
        {
            switch (direction)
            {
                case 0: p.Up = c; c.Down = p; break;
                case 1: p.Down = c; c.Up = p; break;
                case 2: p.Left = c; c.Right = p; break;
                case 3: p.Right = c; c.Left = p; break;
            }
        }

        private static RoomGraph ProcessMapData(RoomGraph start, List<RoomGraph> allRooms, Dictionary<(int x, int y), RoomGraph> occupied)
        {
            var queue = new Queue<RoomGraph>();
            var visited = new HashSet<RoomGraph>();
            foreach (var r in allRooms) r.DistanceFromStart = -1;

            start.DistanceFromStart = 0;
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.Up != null && !visited.Contains(current.Up)) { current.Up.DistanceFromStart = current.DistanceFromStart + 1; visited.Add(current.Up); queue.Enqueue(current.Up); }
                if (current.Down != null && !visited.Contains(current.Down)) { current.Down.DistanceFromStart = current.DistanceFromStart + 1; visited.Add(current.Down); queue.Enqueue(current.Down); }
                if (current.Left != null && !visited.Contains(current.Left)) { current.Left.DistanceFromStart = current.DistanceFromStart + 1; visited.Add(current.Left); queue.Enqueue(current.Left); }
                if (current.Right != null && !visited.Contains(current.Right)) { current.Right.DistanceFromStart = current.DistanceFromStart + 1; visited.Add(current.Right); queue.Enqueue(current.Right); }
            }

            RoomGraph bestRoom = start;
            int maxDist = -1;

            foreach (var r in allRooms)
            {
                // REGLA DE SALIDA PARA EL CARRITO: 
                // 1. Debe ser un callejón (ConnectionCount == 1).
                // 2. La conexión debe ser Vertical (Up o Down).
                // 3. Los espacios físicos LEFT y RIGHT deben estar VACÍOS en el mapa (occupied).
                if (r.ConnectionCount == 1 && r != start)
                {
                    bool isVertical = (r.Up != null || r.Down != null);
                    bool physicalLeftEmpty = !occupied.ContainsKey((r.X - 1, r.Y));
                    bool physicalRightEmpty = !occupied.ContainsKey((r.X + 1, r.Y));

                    if (isVertical && physicalLeftEmpty && physicalRightEmpty)
                    {
                        if (r.DistanceFromStart > maxDist)
                        {
                            maxDist = r.DistanceFromStart;
                            bestRoom = r;
                        }
                    }
                }
            }

            return bestRoom;
        }
        #endregion

        public static (List<RoomGraph>, int) Generate(Random rng, int roomCount)
        {
            var rooms = new List<RoomGraph>();
            var occupied = new Dictionary<(int x, int y), RoomGraph>();

            var start = new RoomGraph(0, 0, 0, RoomType.Start);
            occupied.Add((0, 0), start);
            rooms.Add(start);

            int maxAttempts = 3000;
            int currentAttempt = 0;

            while (rooms.Count < roomCount && currentAttempt < maxAttempts)
            {
                currentAttempt++;
                var parent = rooms[rng.Next(rooms.Count)];
                int direction = rng.Next(4);

                if (parent.RoomType == RoomType.Start && direction == 1) continue;

                var target = GetCoords(parent.X, parent.Y, direction);
                if (occupied.ContainsKey(target)) continue;

                // --- CAMBIO PARA "AIREAR" EL MAPA ---
                // Si tiene 2 o más vecinos, significa que estamos cerrando un ciclo o pegándonos a otra rama.
                // Al ponerlo en >= 2, el mapa se vuelve mucho más ramificado (tipo árbol).
                if (CountNeighbors(target, occupied) >= 2) continue;

                var newRoom = new RoomGraph(rooms.Count, target.x, target.y, RoomType.Connector);
                Link(parent, newRoom, direction);
                occupied.Add(target, newRoom);
                rooms.Add(newRoom);
            }

            var exitRoom = ProcessMapData(start, rooms, occupied);

            if (exitRoom == start) return (rooms, -1);

            exitRoom.RoomType = RoomType.Exit;
            return (rooms, exitRoom.DistanceFromStart);
        }
    }
}