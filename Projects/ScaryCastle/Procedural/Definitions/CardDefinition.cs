using Engendro;
using Microsoft.Xna.Framework;
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

            // Category
            if (element.GetEnum<CardCategory>("category") is not CardCategory category)
                throw new InvalidDataException("Missing card category.");
            else
                this.Category = category;

            // EnergyCost
            this.EnergyCost = element.GetInt32("energyCost", 0);

            // EnergyCostImage
            this.EnergyCostImage = Atlases.UI.GetImage($"CardNumber{EnergyCost}");

            // Value
            this.Value = element.GetInt32("value", 0);

            // Action image
            ActionImage = Action switch
            {
                CardAction.Damage or CardAction.Heal => GetActionImage(Action, Value),
                _ => throw new InvalidOperationException(),
            };

            // CategoryImage
            CategoryImage = Atlases.UI.GetImage($"CardCategory{category}");

            // FrontImage
            FrontImage = Atlases.UI.GetImage($"Card{Category}");

            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

        #region Private members

        // GetActionImage
        private static AtlasImage GetActionImage(CardAction action, int value)
        {
            var prefix = "Heart";

            if (action == CardAction.Shield)
                prefix += "Blue";

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

        // Apply
        public int Apply(Actor source, Actor target)
        {
            switch (Action)
            {
                // Damage
                case CardAction.Damage:
                    target.TakeDamage(source, AttackType.Contact, DamageType.None, Value, ImpactWordName.None, Vector2.Zero);
                    break;

                // Heal
                case CardAction.Heal:
                    // La cura siempre es al Source (a uno mismo)
                    source.HP += Value;
                    break;

                case CardAction.Shield:
                    // El escudo se aplica al Source
                    // Asumiendo que tenés una propiedad 'Block' o 'Armor' en Actor
                    // source.Block += finalValue; 
                    break;
            }

            return Value;
        }

        // Category
        public CardCategory Category { get; }

        // CategoryImage
        public AtlasImage CategoryImage { get; }

        // EnergyCost
        public int EnergyCost { get; }

        // EnergyCostImage
        public AtlasImage EnergyCostImage { get; }

        // FrontImage
        public AtlasImage FrontImage { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }

        // Value
        public int Value { get; }
    }
}
