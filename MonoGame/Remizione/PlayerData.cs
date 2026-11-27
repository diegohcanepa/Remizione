using System;

namespace Remizione
{
    /// <summary>
    /// PlayerData
    /// </summary>
    public sealed class PlayerData
    {
        private readonly GameSession session;

        // Constructor
        public PlayerData(GameSession session)
        {
            this.session = session;
            Inventory = new(session);
        }

        // Ashes
        public int Ashes
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Max(0, value);
                    session.AshesEnabled = true;
                }
            }
        }

        // Crafts
        public int Crafts
        {
            get;
            set
            {
                if (value != field)
                    field = Math.Max(0, value);
            }
        }

        // Inventory
        public ItemContainer Inventory { get; }
    }
}
