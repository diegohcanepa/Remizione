using Engendro;
using Engendro.Audio;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ItemDefinition
    /// </summary>
    public sealed class ItemDefinition
    {
        private static readonly Dictionary<string, ItemDefinition> data = [];
        private readonly List<EffectDefinition> effects = [];
        private static bool loaded;

        #region Constructor

        // Constructor
        private ItemDefinition(JsonElement element)
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
            Category = element.GetEnum("category", ItemCategory.Misc);

            // ConsumptionInterval
            ConsumptionInterval = element.GetInt32("consumptionInterval", 0);
            if (ConsumptionInterval < 0)
                ConsumptionInterval = 0;

            // ConsumptionType
            ConsumptionType = element.GetEnum("consumptionType", ConsumptionType.None);
            
            // Damage
            DiceExpression? damage = element.GetObject("hp", value => new DiceExpression(value));

            // DamageType
            var damageType = element.GetEnum<DamageType>("damageType", DamageType.None);

            // Durability
            Durability = element.GetFloat("durability", 0);

            // DurabilityCost
            DurabilityCost = element.GetFloat("durabilityCost", 0);

            // HP
            DiceExpression? hp = element.GetObject("hp", value => new DiceExpression(value));

            // ImpactWord
            var impactWord = element.GetEnum("impactWord", ImpactWordName.None);

            // IsStackable
            IsStackable = element.GetBool("isStackable", false);

            // LuckBonus
            Ratio luckBonus = element.GetFloat("luckBonus", 0);

            // PickupSound
            PickupSound = element.GetObject("pickupSound", Sound.Get);

            // Quality
            Quality = element.GetInt32("quality", 0);

            // Range
            Range = element.GetInt32("range", 0);

            // Realm
            Realm = element.GetEnum("realm", Realm.Earthly);

            // SkillChance
            SkillChance = element.GetInt32("skillChance", 0);

            // Sound
            Sound? sound = element.GetObject("sound", Sound.Get);

            // SpawnWeight
            SpawnWeight = element.GetFloat("spawnWeight", 1);

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

            if (element.TryGetProperty("effects", out JsonElement effectsArray))
            {
                foreach (var effectJson in effectsArray.EnumerateArray())
                {
                    effects.Add(new(effectJson));
                }
            }

            data.Add(Name, this);

            Effects = effects.AsReadOnly();
        }

        #endregion

        #region Static members

        // All
        public static IEnumerable<ItemDefinition> All => data.Values;

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

            foreach (var item in data.Values)
            {
                if (item.Category == category)
                    result.Add(item);
            }

            return result;
        }

        // GetItems
        public static List<ItemDefinition> GetItems(Realm realm)
        {
            var result = new List<ItemDefinition>();

            foreach (var item in data.Values)
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

            Utils.LoadJsonData(fileName, element => new ItemDefinition(element));

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

        // Effects
        public ReadOnlyCollection<EffectDefinition> Effects { get; }

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
