using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// Deck
    /// </summary>
    public sealed class Deck
    {
        private readonly GameSession session;

        // Constructor
        public Deck(GameSession session)
        {
            this.session = session;
        }

        // Count
        public int Count => session.Inventory.Count;
    }
}
