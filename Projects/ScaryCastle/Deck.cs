using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// Deck
    /// </summary>
    public sealed class Deck
    {
        private readonly List<Card> cards = [];
        private readonly GameSession session;

        // Constructor
        public Deck(GameSession session)
        {
            this.session = session;
            this.Cards = cards.AsReadOnly();
        }

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Count
        public int Count => session.Inventory.Count;

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
        }

        // Capacity
        public int Capacity { get; set; } = 5;

        // Cards
        public ReadOnlyCollection<Card> Cards { get; }
    }
}
