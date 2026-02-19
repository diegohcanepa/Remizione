using Engendro;
using Engendro.Audio;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ItemDefinition
    /// </summary>
    public sealed class ItemDefinition : Definition
    {
        #region Constructor

        // Constructor
        private ItemDefinition(JsonElement element)
            : base(element)
        {
            // AreaRange
            AreaRange = element.GetEnum("areaRange", EffectAreaRange.None);

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

            // ExecutionDelay
            ExecutionDelay = element.GetInt32("executionDelay", 0);

            // HP
            DiceExpression? hp = element.GetObject("hp", value => new DiceExpression(value));

            // ImpactWord
            var impactWord = element.GetEnum("impactWord", ImpactWordName.None);

            // InventoryCategory
            InventoryCategory = element.GetEnum("inventoryCategory", InventoryCategory.Common);

            // IsStackable
            IsStackable = element.GetBool("isStackable", false);

            // ItemUsageMode
            UsageMode = element.GetEnum("usageMode", ItemUsageMode.Default);

            // PickupSound
            PickupSound = element.GetObject("pickupSound", Sound.Get) ?? Sound.Get(SoundNames.PickupGeneric);

            // Quality
            Quality = element.GetInt32("quality", 0);

            // Realm
            Realm = element.GetEnum("realm", Realm.Earthly);

            SelfTarget = element.GetBool("selfTarget", false);

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

            Definitions.Add(this);
        }

        #endregion

        // AreaRange
        public EffectAreaRange AreaRange { get; }

        // Category
        public ItemCategory Category { get; }

        // ConsumptionInterval
        public int ConsumptionInterval { get; }

        // ConsumptionType
        public ConsumptionType ConsumptionType { get; }

        // Definitions
        public static ItemDefinitionContainer Definitions { get; } = new(element => new ItemDefinition(element));

        // Durability
        public Ratio Durability { get; }

        // DurabilityCost
        public float DurabilityCost { get; }

        // ExecutionDelay
        public int ExecutionDelay { get; }

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

        // SelfTarget
        public bool SelfTarget { get; }

        // SkillChance
        public int SkillChance { get; }

        // UsageMode
        public ItemUsageMode UsageMode { get; }
    }
}
