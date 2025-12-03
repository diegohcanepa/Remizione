using Engendro;
using Remizione.Procedural.Graphs;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RunGraphGenerator
    /// </summary>
    public static class RunGraphGenerator
    {
        private static int roomId;

        // Generate
        public static RunGraph Generate(GameSession session, RunGraphGeneratorSettings settings)
        {
            var random = new Random(settings.Seed);

            var entryRooms = new List<RoomGraph>();
            roomId = 0;

            for (int i = 0; i < settings.PathCount; i++)
            {
                int length = random.Next(settings.MinLength, settings.MaxLength + 1);
                var entryRoom = GeneratePath(i, length, settings);
                entryRooms.Add(entryRoom);
            }

            return new RunGraph(session, entryRooms, random, settings.Pools, settings.Tags);
        }

        #region Private members

        // CreateRoom
        private static RoomGraph CreateRoom(int pathIndex, bool isRoot)
        {
            roomId++;
            var result = new RoomGraph(roomId, isRoot, pathIndex);
            return result;
        }

        // GeneratePath
        private static RoomGraph GeneratePath(int pathIndex, int length, RunGraphGeneratorSettings settings)
        {
            RoomGraph first = CreateRoom(pathIndex, true);
            RoomGraph prev = first;

            // columna principal
            for (int i = 1; i < length; i++)
            {
                var next = CreateRoom(pathIndex, false);
                prev.Up = next;
                next.Down = prev;
                prev = next;
            }

            // generar side dead-ends
            RoomGraph? cur = first;
            int countForPath = 0;

            while (cur != null)
            {
                for (int s = 0; s < settings.MaxSidePerRoom; s++)
                {
                    if (countForPath >= settings.MaxSidePerPath)
                        break;

                    // Left connection
                    if (DiceExpression.Dice100.Roll() <= settings.SideChancePercent)
                    {
                        if (cur.Left == null)
                        {
                            var side = CreateRoom(pathIndex, false);
                            cur.Left = side;
                            side.Right = cur;
                            countForPath++;
                        }
                    }

                    // Right connection
                    if (DiceExpression.Dice100.Roll() <= settings.SideChancePercent)
                    {
                        if (cur.Right == null)
                        {
                            var side = CreateRoom(pathIndex, false);
                            cur.Right = side;
                            side.Left = cur;
                            countForPath++;
                        }
                    }
                }

                cur = cur.Up;
            }

            return first;
        }

        #endregion
    }
}
