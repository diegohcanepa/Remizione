using System;

namespace Remizione
{
    /// <summary>
    /// PlayerData
    /// </summary>
    public sealed class PlayerData
    {
        // Constructor
        public PlayerData(GameSession session)
        {
            Inventory = new(session);
        }

        // Ashes
        public int Ashes
        {
            get;
            set
            {
                if (value != field)
                    field = Math.Max(0, value);
            }
        }

        // Bonfires
        public int Bonfires { get; set; }

        // Inventory
        public ItemContainer Inventory { get; }

        // IsSafe
        public bool IsSafe { get; set; } = true;
    }
}
