using Engendro;
using ScaryCastle.Procedural;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CardDefinition
    /// </summary>
    public sealed class CardDefinition : Definition
    {
        private static readonly Dictionary<string, CardDefinition> data = [];
        private static readonly List<CardDefinition> dataList = [];

        #region Constructor

        // Constructor
        public CardDefinition(JsonElement element)
            : base(element)
        {
            // Action
            if (element.GetEnum<CardAction>("action") is not CardAction action)
                throw new InvalidDataException("Missing card action.");
            else
                this.Action = action;

            // BaseValue
            this.BaseValue = element.GetInt32("baseValue", 0);

            // BonusValue
            this.BonusValue = element.GetInt32("bonusValue", 0);
            CodeContract.ValidRange(BonusValue, 0, 5, nameof(BonusValue));

            // Category
            if (element.GetEnum<CardCategory>("category") is not CardCategory category)
                throw new InvalidDataException("Missing card category.");
            else
                this.Category = category;

            // DiceThreshold
            this.DiceThreshold = element.GetInt32("diceThreshold", 0);
            CodeContract.ValidRange(DiceThreshold, 0, 5, nameof(DiceThreshold));

            BackImage = Atlases.UI.GetImage($"CardBack");
          
            switch (Action)
            {
                case CardAction.Damage:
                case CardAction.Heal:
                    ActionImage = GetHeartImage(Action, BaseValue);
                    break;

                default:
                    throw new InvalidOperationException();
            };

            // BonusValueImage
            if (BonusValue != 0)
                BonusValueImage = GetHeartImage(Action, BonusValue);

            // CategoryImage
            CategoryImage = Atlases.UI.GetImage($"CardCategory{category}");

            // DiceThresholdImage
            if (DiceThreshold != 0)
                DiceThresholdImage = Atlases.UI.GetImage($"CardNumber{DiceThreshold}");

            // FrontImage
            FrontImage = Atlases.UI.GetImage($"Card{Category}");

            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

        #region Private members

        // GetHeartImage
        private static AtlasImage GetHeartImage(CardAction action, int value)
        {
            var prefix = "Heart";

            if (action == CardAction.Damage)
                prefix += "Black";

            else if (action == CardAction.Shield)
                prefix += "Blue";

            else if (action == CardAction.Heal)
                prefix += "Red";

            if (value == 1)
                prefix += "Half";
            else
                prefix += "Full";

            return Atlases.UI.GetImage(prefix) ?? throw new InvalidOperationException();
        }

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<CardDefinition> All { get; } = new(dataList);

        // Find
        public static CardDefinition? Find(string name)
        {
            return data.TryGetValue(name, out var result) ? result : null;
        }

        // Get
        public static CardDefinition Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"{nameof(CardDefinition)} '{name}' not found.");
        }

        // Load
        public static void Load(string fileName)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            Utils.LoadJsonData(fileName, element => new CardDefinition(element));
        }

        #endregion

        // Action
        public CardAction Action { get; }

        // ActionImage
        public AtlasImage ActionImage { get; }

        // BackImage
        public AtlasImage BackImage { get; }

        // BaseValue
        public int BaseValue { get; }

        // BonusValue
        public int BonusValue { get; set; }

        // BonusValueImage
        public AtlasImage? BonusValueImage { get; }

        // Category
        public CardCategory Category { get; }

        // CategoryImage
        public AtlasImage CategoryImage { get; }

        // DiceThreshold
        public int DiceThreshold { get; }

        // DiceThresholdImage
        public AtlasImage? DiceThresholdImage { get; }

        // FrontImage
        public AtlasImage FrontImage { get; }

        // HasBonus
        public bool HasBonus
        {
            get
            {
                return DiceThreshold > 0 && BonusValue > 0;
            }
        }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
