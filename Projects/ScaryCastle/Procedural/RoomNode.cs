using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// RoomNode
    /// </summary>
    public sealed class RoomNode
    {
        // Constructor
        public RoomNode(int index, Point gridPosition)
        {
            this.Index = index;
            this.GridPosition = gridPosition;
        }

        // Category
        public RoomCategory Category { get; set; } = RoomCategory.Standard;

        // CoinCount
        public int CoinCount { get; set; }

        // ConnectionCount
        public int ConnectionCount()
        {
            return (Up != null ? 1 : 0) + (Down != null ? 1 : 0) +
                   (Left != null ? 1 : 0) + (Right != null ? 1 : 0);
        }

        // Definition
        public RoomDefinition Definition { get; set; } = null!;

        // Down
        public RoomNode? Down { get; set; }

        // Fits
        public bool Fits(RoomDefinition def)
        {
            var needsUp = Up != null;
            var needsDown = Down != null;
            var needsLeft = Left != null;
            var needsRight = Right != null;

            // Filtro 1: Debe tener las puertas que la topología le exige
            if (needsUp && def.DoorUp == null)
                return false;

            if (needsDown && def.DoorDown == null)
                return false;

            if (needsLeft && def.DoorLeft == null)
                return false;

            if (needsRight && def.DoorRight == null)
                return false;

            // Filtro 2: El Plan A Rígido (exactMatch)
            // Si la pieza exige exactitud, no puede tener puertas donde la topología dice que hay pared.
            if (def.ExactMatch)
            {
                if (!needsUp && def.DoorUp != null)
                    return false;

                if (!needsDown && def.DoorDown != null)
                    return false;

                if (!needsLeft && def.DoorLeft != null)
                    return false;

                if (!needsRight && def.DoorRight != null)
                    return false;
            }

            return true;
        }

        // GridPosition
        public Point GridPosition { get; }

        // Index
        public int Index { get; }

        // Left
        public RoomNode? Left { get; set; }

        // LockedDoors
        public Dictionary<DoorDirection, LockType> LockedDoors { get; } = [];

        // LootCount
        public int LootCount { get; set; }

        // PendingBronzeKeys
        public int PendingBronzeKeys { get; set; }

        // RideRoom
        public RideRoom RideRoom { get; set; } = null!;

        // Right
        public RoomNode? Right { get; set; }

        // TopographicDifficulty
        public Difficulty TopographicDifficulty { get; set; }

        // ToString
        public override string ToString()
        {
            return $"[Room_{Category}_{Index} ({GridPosition.X},{GridPosition.Y})]";
        }

        // Up
        public RoomNode? Up { get; set; }

        // Visited
        public bool Visited { get; set; }
    }
}