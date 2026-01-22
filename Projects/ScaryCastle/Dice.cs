using System;
using System.Collections.Generic;
using System.Text;

namespace ScaryCastle
{
    /// <summary>
    /// DiceInfo
    /// </summary>
    public sealed class DiceInfo
    {
        // Constructor
        public DiceInfo(DiceBag diceBag, CardDefinition cardDefinition)
        {
            this.DiceBag = diceBag;
            this.CardDefinition = cardDefinition;
        }

        // DiceBag
        public DiceBag DiceBag { get; }

        // Definition
        public CardDefinition CardDefinition { get; }
    }
}
