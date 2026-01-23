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

        // Constructor
        public Deck(GameSession session)
        {
            this.Session = session;
            this.Cards = cards.AsReadOnly();
            Add(CardDefinition.Get("CardTest"));
        }

        // Add
        public Card? Add(CardDefinition definition)
        {
            if (IsFull)
                return null;

            var card = new Card(this, definition);
            cards.Add(card);

            Invalidate();

            return card;
        }

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Count
        public int Count => Session.Inventory.Count;

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
        }

        // Capacity
        public int Capacity { get; set; } = 5;

        // Cards
        public ReadOnlyCollection<Card> Cards { get; }

        // Find
        public Card? Find(string name)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                if (cards[i].Name == name)
                    return cards[i];
            }

            return null;
        }

        // GetCard
        public Card GetCard(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"Card '{name}' not found.");
        }

        // GetCards
        public Card[] GetCards()
        {
            return [.. cards];
        }

        // GetCards
        public Card[] GetCards(CardCategory category)
        {
            var result = new List<Card>();

            for (var i = 0; i < cards.Count; i++)
            {
                if (cards[i].Definition.Category == category)
                    result.Add(cards[i]);
            }

            return [.. result];
        }

        // Indexer
        public Card this[int index] => cards[index];

        // IsEmpty
        public bool IsEmpty => cards.Count == 0;

        // IsFull
        public bool IsFull => cards.Count >= Capacity;

        // Session
        public GameSession Session { get; }
    }
}
