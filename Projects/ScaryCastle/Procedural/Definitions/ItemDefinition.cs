using Engendro;
using Engendro.Audio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ItemDefinition
    /// </summary>
    public sealed class ItemDefinition : Definition
    {
        private static readonly Dictionary<string, ItemDefinition> data = [];
        private static readonly List<ItemDefinition> dataList = [];

        #region Constructor

        // Constructor
        private ItemDefinition(JsonElement element)
            : base(element)
        {
            // Category
            Category = element.GetEnum("category", ItemCategory.Misc);

            // ConsumptionInterval
            ConsumptionInterval = element.GetInt32("consumptionInterval", 0);
            if (ConsumptionInterval < 0)
                ConsumptionInterval = 0;

            // ConsumptionType
            ConsumptionType = element.GetEnum("consumptionType", ConsumptionType.Quantity);

            // Damage
            DiceExpression? damage = element.GetObject("hp", value => new DiceExpression(value));

            // Durability
            Durability = element.GetFloat("durability", 0);

            // DurabilityCost
            DurabilityCost = element.GetFloat("durabilityCost", 0);

            // HP
            DiceExpression? hp = element.GetObject("hp", value => new DiceExpression(value));

            // ImpactWord
            var impactWord = element.GetEnum("impactWord", ImpactWordName.None);

            // InventoryCategory
            InventoryCategory = element.GetEnum<InventoryCategory>("inventoryCategory", InventoryCategory.Common);

            // IsStackable
            IsStackable = element.GetBool("isStackable", false);

            // PickupSound
            PickupSound = element.GetObject("pickupSound", Sound.Get) ?? Sound.Get(SoundNames.PickupGeneric);

            // Quality
            Quality = element.GetInt32("quality", 0);

            // Realm
            Realm = element.GetEnum("realm", Realm.Earthly);

            // SkillChance
            SkillChance = element.GetInt32("skillChance", 0);

            // Sound
            Sound? sound = element.GetObject("sound", Sound.Get);

            this.LocalizedDescription = Localization.GetItemDescription(this);
            this.LocalizedDisplayName = Localization.GetItemName(this);
            this.Image = Atlases.UI.FindImage(Name);

            this.Price = Quality switch
            {
                0 or 1 => 5,  // Items básicos o consumibles
                2 or 3 => 10, // Herramientas y gadgets de nivel medio
                4 or 5 => 15, // Items poderosos o de alta calidad
                _ => 5
            };

            IsPassive = EffectDescriptors.Count > 0;

            for (var i = 0; i < EffectDescriptors.Count; i++)
            {
                if (!EffectDescriptors[i].IsPassive)
                {
                    IsPassive = false;
                    break;
                }
            }

            data.Add(Name, this);
            dataList.Add(this);
        }

        #endregion

        #region Static members

        // All
        public static ReadOnlyCollection<ItemDefinition> All { get; } = new(dataList);

        // Find
        public static ItemDefinition? Find(string name)
        {
            return data.TryGetValue(name, out var result) ? result : null;
        }

        // Get
        public static ItemDefinition Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"{nameof(ItemDefinition)} '{name}' not found.");
        }

        // GetItems
        public static List<ItemDefinition> GetItems(ItemCategory category)
        {
            var result = new List<ItemDefinition>();

            for (var i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].Category == category)
                    result.Add(dataList[i]);
            }

            return result;
        }

        // GetItems
        public static List<ItemDefinition> GetItems(Realm realm)
        {
            var result = new List<ItemDefinition>();

            for (var i = 0; i < dataList.Count; i++)
            {
                if (dataList[i].Realm == realm)
                    result.Add(dataList[i]);
            }

            return result;
        }

        // Load
        public static void Load(string fileName)
        {
            if (data.Count > 0)
                throw new InvalidOperationException("Data already loaded.");

            Utils.LoadJsonData(fileName, element => new ItemDefinition(element));
        }

        #endregion

        // Category
        public ItemCategory Category { get; }

        // ConsumptionInterval
        public int ConsumptionInterval { get; }

        // ConsumptionType
        public ConsumptionType ConsumptionType { get; }

        // Durability
        public Ratio Durability { get; }

        // DurabilityCost
        public float DurabilityCost { get; }

        // Image
        public AtlasImage? Image { get; }

        // InventoryCategory
        public InventoryCategory InventoryCategory { get; }

        // IsPassive
        public bool IsPassive { get; }

        // IsStackable
        public bool IsStackable { get; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; }

        // PickupSound
        public Sound PickupSound { get; }

        // Price
        public int Price { get; }

        // Quality
        public int Quality { get; }

        // Range
        public int Range { get; }

        // Realm
        public Realm Realm { get; }

        // SkillChance
        public int SkillChance { get; }
    }
}
