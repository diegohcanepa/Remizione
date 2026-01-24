using Engendro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// Deck
    /// </summary>
    public sealed class Deck : Collection<Card>
    {
        private readonly List<Card> discardPile = [];
        private readonly List<Card> drawPile = [];

        // Constructor
        public Deck(GameSession session)
            : base()
        {
            this.Session = session;
            this.DiscardPile = discardPile.AsReadOnly();
            this.DrawPile = drawPile.AsReadOnly();
        }

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            discardPile.Clear();
            drawPile.Clear();
            Invalidate();
        }

        // InsertItem
        protected override void InsertItem(int index, Card item)
        {
            if (IsFull)
                throw new InvalidOperationException("Deck is full.");

            base.InsertItem(index, item);

            drawPile.Add(item);

            Invalidate();
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            var card = this[index];
            base.RemoveItem(index);
            discardPile.Remove(card);
            drawPile.Remove(card);
            Invalidate();
        }

        #endregion

        // Capacity
        public int Capacity { get; set; } = 5;

        // ContentVersion
        public int ContentVersion { get; private set; }

        // Draw
        public Card? Draw()
        {
            if (drawPile.Count == 0)
                return null;

            var card = drawPile[0];
            drawPile.RemoveAt(0);
            Invalidate();
            
            return card;
        }

        // Discard
        public bool Discard(Card card)
        {
            if (Contains(card) && !discardPile.Contains(card))
            {
                drawPile.Remove(card);
                discardPile.Add(card);
                Invalidate();
                return true;
            }

            return false;
        }

        // DiscardPile
        public ReadOnlyCollection<Card> DiscardPile { get; }

        // DrawPile
        public ReadOnlyCollection<Card> DrawPile { get; }

        // Find
        public Card? Find(string name)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                    return this[i];
            }

            return null;
        }

        // Get
        public Card Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"Card '{name}' not found.");
        }

        // Invalidate
        public void Invalidate()
        {
            unchecked { ContentVersion++; }
        }

        // IsEmpty
        public bool IsEmpty => Count == 0;

        // IsFull
        public bool IsFull => Count == Capacity;

        // LoadState
        public void LoadState(string data)
        {
            void AddCards(IEnumerable<string> cardNames, IList<Card> targetList, bool linkCards)
            {
                targetList.Clear();

                foreach (var cardName in cardNames)
                {
                    if (linkCards)
                    {
                        if (Find(cardName) is Card existingCard)
                            targetList.Add(existingCard);
                    }
                    else
                    {
                        targetList.Add(new Card(Session.Game, cardName));
                    }
                }
            }

            Clear();

            if (string.IsNullOrEmpty(data))
                return;

            var cardGroups = data.Split(';');

            if (cardGroups.Length > 0)
            {
                // Deck
                AddCards(cardGroups[0].Split(','), this, false);

                // Draw pile
                if (cardGroups.Length > 1)
                    AddCards(cardGroups[1].Split(','), drawPile, true);

                // Discard pile
                if (cardGroups.Length > 2)
                    AddCards(cardGroups[2].Split(','), discardPile, true);
            }
        }

        // Remove
        public bool Remove(string name)
        {
            return Find(name) is Card card && Remove(card);
        }

        // SaveState
        public string SaveState()
        {
            string Serialize(IEnumerable<Card> cards)
            {
                var result = new List<string>();

                foreach (var card in cards)
                {
                    result.Add(card.Name);
                }

                return string.Join(",", result);
            }

            var result = new List<string>
            {
                Serialize(this),
                Serialize(drawPile),
                Serialize(discardPile)
            };

            return string.Join(";", result);
        }

        // Session
        public GameSession Session { get; }

        // Shuffle
        public void Shuffle()
        {
            discardPile.Clear();
            drawPile.Clear();
            drawPile.AddRange(this);
            drawPile.Shuffle();
            Invalidate();
        }
    }
}
