using Engendro;
using Engendro.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// MetaItem
    /// </summary>
    public sealed class MetaItem
    {
        private static bool loaded;
        private static readonly Dictionary<string, MetaItem> metaItems = [];

        #region Constructor

        // Constructor
        private MetaItem(JsonElement element)
        {
            // Name
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);

            Utils.AssertName(Name, this);

            // Name cannot be a realm 
            if (Enum.IsDefined(typeof(Realm), Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is an item realm.");

            // Name cannot be a category
            if (Enum.IsDefined(typeof(ItemCategory), Name))
                throw new InvalidOperationException($"The name '{Name}' cannot be used because it is an item category.");

            // Category
            if (element.TryGetProperty("category", out JsonElement categoryElement) && categoryElement.GetString() is string categoryValue)
                Category = Enum.Parse<ItemCategory>(categoryValue);

            // ConsumptionInterval
            if (element.TryGetProperty("consumptionInterval", out JsonElement consumptionIntervalElement))
                ConsumptionInterval = int.Clamp(consumptionIntervalElement.GetInt32(), 0, consumptionIntervalElement.GetInt32());

            // ConsumptionType
            if (element.TryGetProperty("consumptionType", out JsonElement consumptionTypeElement) && consumptionTypeElement.GetString() is string consumptionTypeValue)
                ConsumptionType = Enum.Parse<ConsumptionType>(consumptionTypeValue);

            // Damage
            DiceExpression? damage = null;
            if (element.TryGetProperty("damage", out JsonElement damageElement) && damageElement.GetString() is string damageValue)
                damage = new(damageValue);

            // DamageType
            var damageType = DamageType.None;
            if (element.TryGetProperty("damageType", out JsonElement damageTypeElement) && damageTypeElement.GetString() is string damageTypeValue)
                damageType = Enum.Parse<DamageType>(damageTypeValue);

            // Durability
            if (element.TryGetProperty("durability", out JsonElement durabilityElement))
                Durability = durabilityElement.GetSingle();

            // DurabilityCost
            if (element.TryGetProperty("durabilityCost", out JsonElement durabilityCostElement))
                DurabilityCost = durabilityCostElement.GetSingle();

            // HP
            DiceExpression? hp = null;
            if (element.TryGetProperty("hp", out JsonElement hpElement) && hpElement.GetString() is string hpValue)
                hp = new(hpValue);

            // ImpactWord
            var impactWord = ImpactWordName.None;
            if (element.TryGetProperty("impactWord", out JsonElement impactWordElement) && impactWordElement.GetString() is string impactWordValue)
                impactWord = Enum.Parse<ImpactWordName>(impactWordValue);

            // IsStackable
            if (element.TryGetProperty("isStackable", out JsonElement isStackableElement))
                IsStackable = isStackableElement.GetBoolean();

            // LuckBonus
            Ratio luckBonus = 0;
            if (element.TryGetProperty("luckBonus", out JsonElement luckBonusElement))
                luckBonus = luckBonusElement.GetSingle();

            // PickupSound
            if (element.TryGetProperty("pickupSound", out JsonElement pickupSoundElement) && pickupSoundElement.GetString() is string pickupSoundValue)
                PickupSound = Sound.Get(pickupSoundValue);

            // Price
            if (element.TryGetProperty("price", out JsonElement priceElement))
                Price = priceElement.GetInt32();

            // Quality
            if (element.TryGetProperty("quality", out JsonElement qualityElement))
                Quality = qualityElement.GetInt32();

            // Range
            if (element.TryGetProperty("range", out JsonElement rangeElement))
                Range = rangeElement.GetInt32();

            // Realm
            if (element.TryGetProperty("realm", out JsonElement realmElement) && realmElement.GetString() is string realmValue)
                Realm = Enum.Parse<Realm>(realmValue);

            // SkillChance
            if (element.TryGetProperty("skillChance", out JsonElement skillChanceElement))
                SkillChance = skillChanceElement.GetInt32();

            // Sound
            Sound? sound = null;
            if (element.TryGetProperty("sound", out JsonElement soundElement) && soundElement.GetString() is string soundValue)
                sound = Sound.Get(soundValue);

            // SpawnWeight
            SpawnWeight = 1;
            if (element.TryGetProperty("spawnWeight", out JsonElement spawnWeightElement))
                SpawnWeight = spawnWeightElement.GetSingle();

            // Effect
            this.Effect = new($"<{Name} Effect>")
            {
                Damage = damage,
                DamageType = damageType,
                HP = hp,
                ImpactWord = impactWord,
                LuckBonus = luckBonus,
                Sound = sound
            };

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

            metaItems.Add(Name, this);
        }

        #endregion

        #region Static members

        // AllItems
        public static IEnumerable<MetaItem> AllItems => metaItems.Values;

        // Find
        public static MetaItem? Find(string name)
        {
            return metaItems.TryGetValue(name, out var result) ? result : null;
        }

        // Get
        public static MetaItem Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"{nameof(MetaItem)} '{name}' not found.");
        }

        // GetItems
        public static List<MetaItem> GetItems(ItemCategory category)
        {
            var result = new List<MetaItem>();

            foreach (var item in metaItems.Values)
            {
                if (item.Category == category)
                    result.Add(item);
            }

            return result;
        }

        // GetItems
        public static List<MetaItem> GetItems(Realm realm)
        {
            var result = new List<MetaItem>();

            foreach (var item in metaItems.Values)
            {
                if (item.Realm == realm)
                    result.Add(item);
            }

            return result;
        }

        // Load
        public static void Load(string fileName)
        {
            if (loaded)
                throw new InvalidOperationException("Data is already loaded.");

            Utils.LoadJsonData(fileName, element => new MetaItem(element));

            loaded = true;
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

        // Effect
        public EffectDefinition Effect { get; }

        // Image
        public AtlasImage? Image { get; }

        // IsStackable
        public bool IsStackable { get; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; }

        // Name
        public string Name { get; }

        // PickupSound
        public Sound? PickupSound { get; }

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

        // SpawnWeight
        public float SpawnWeight { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
