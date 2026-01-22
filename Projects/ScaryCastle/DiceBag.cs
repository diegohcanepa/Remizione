using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// DiceBag
    /// </summary>
    public sealed class DiceBag
    {
        private readonly List<Dice> dice = [];
        private readonly GameSession session;

        // Constructor
        public DiceBag(GameSession session)
        {
            this.session = session;
            this.Cards = dice.AsReadOnly();
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
        public ReadOnlyCollection<Dice> Cards { get; }
    }
}
