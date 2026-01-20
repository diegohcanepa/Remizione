using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// Card
    /// </summary>
    public sealed class Card
    {
        // Constructor
        public Card(Deck deck, CardDefinition cardDefinition)
        {
            this.Deck = deck;
            this.CardDefinition = cardDefinition;
        }

        // Deck
        public Deck Deck { get; }

        // Definition
        public CardDefinition CardDefinition { get; }
    }
}
