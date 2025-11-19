using System;

namespace Remizione
{
    /// <summary>
    /// RunDescriptorGenerator
    /// </summary>
    public sealed class RunDescriptorGenerator
    {
        private readonly Random random;
        private int roomCount;

        // Constructor
        public RunDescriptorGenerator(int seed)
        {
            random = new Random(seed);
        }

        // Generate
        public RunDescriptor Generate(int paths)
        {
            var run = new RunDescriptor(paths);
            roomCount = 0;

            for (int i = 0; i < paths; i++)
            {
                int length = RandomRange(MinLength, MaxLength + 1);
                var root = GenerateMainPath(length);
                run.Paths[i] = root;
            }

            return run;
        }

        #region Private members

        // CreateRoom
        private RoomDescriptor CreateRoom(RideRoomKind kind)
        {
            var result = new RoomDescriptor(roomCount, kind);
            roomCount++;
            return result;
        }

        // GenerateMainPath
        private RoomDescriptor GenerateMainPath(int length)
        {
            RoomDescriptor first = CreateRoom(RideRoomKind.Default);
            RoomDescriptor prev = first;

            // columna principal
            for (int i = 1; i < length; i++)
            {
                var next = CreateRoom(RideRoomKind.Default);
                prev.Up = next;
                next.Down = prev;
                prev = next;
            }

            // generar side dead-ends
            RoomDescriptor? cur = first;
            int countForPath = 0;

            while (cur != null)
            {
                for (int s = 0; s < MaxSidePerRoom; s++)
                {
                    if (countForPath >= MaxSidePerPath) break;

                    if (Roll(SideChancePercent))
                    {
                        if (cur.Left == null)
                        {
                            var side = CreateRoom(RideRoomKind.Default);
                            cur.Left = side;
                            side.Right = cur;
                            countForPath++;
                        }
                        else if (cur.Right == null)
                        {
                            var side = CreateRoom(RideRoomKind.Default);
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

        // RandomRange
        private int RandomRange(int minInclusive, int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }

        // Roll
        private bool Roll(int percent)
        {
            return RandomRange(0, 100) < percent;
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
