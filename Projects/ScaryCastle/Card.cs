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
        public Card(Deck diceBag, CardDefinition cardDefinition)
        {
            this.DiceBag = diceBag;
            this.CardDefinition = cardDefinition;
        }

        // DiceBag
        public Deck DiceBag { get; }

        // Definition
        public CardDefinition CardDefinition { get; }
    }
}
