using Engendro;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RunGraphGenerator
    /// </summary>
    public sealed class RunGraphGenerator
    {
        private readonly Random random;
        private int roomId;

        // Constructor
        public RunGraphGenerator(int seed)
        {
            random = new Random(seed);
        }

        // Generate
        public RunGraph Generate(int pathCount)
        {
            var entryRooms = new List<RoomGraph>();
            roomId = 0;

            for (int i = 0; i < pathCount; i++)
            {
                int length = random.Next(MinLength, MaxLength + 1);
                var entryRoom = GeneratePath(i, length);
                entryRooms.Add(entryRoom);
            }

            return new RunGraph(entryRooms);
        }

        #region Private members

        // CreateRoom
        private RoomGraph CreateRoom(RideRoomKind kind, int pathIndex, bool isRoot)
        {
            var result = new RoomGraph(roomId, isRoot, pathIndex, kind);
            roomId++;
            return result;
        }

        // GeneratePath
        private RoomGraph GeneratePath(int pathIndex, int length)
        {
            RoomGraph first = CreateRoom(RideRoomKind.Default, pathIndex, true);
            RoomGraph prev = first;

            // columna principal
            for (int i = 1; i < length; i++)
            {
                var next = CreateRoom(RideRoomKind.Default, pathIndex, false);
                prev.Up = next;
                next.Down = prev;
                prev = next;
            }

            // generar side dead-ends
            RoomGraph? cur = first;
            int countForPath = 0;

            while (cur != null)
            {
                for (int s = 0; s < MaxSidePerRoom; s++)
                {
                    if (countForPath >= MaxSidePerPath)
                        break;

                    // Left connection
                    if (DiceExpression.Dice100.Roll() <= SideChancePercent)
                    {
                        if (cur.Left == null)
                        {
                            var side = CreateRoom(RideRoomKind.Default, pathIndex, false);
                            cur.Left = side;
                            side.Right = cur;
                            countForPath++;
                        }
                    }

                    // Right connection
                    if (DiceExpression.Dice100.Roll() <= SideChancePercent)
                    {
                        if (cur.Right == null)
                        {
                            var side = CreateRoom(RideRoomKind.Default, pathIndex, false);
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

        // MaxLength
        public int MaxLength { get; set; } = 7;

        // MaxSidePerRoom
        public int MaxSidePerRoom { get; set; } = 1;

        // MaxSidePerPath
        public int MaxSidePerPath { get; set; } = int.MaxValue;

        // MinLength
        public int MinLength { get; set; } = 4;

        // SideChancePercent
        public int SideChancePercent { get; set; } = 50;
    }
}
