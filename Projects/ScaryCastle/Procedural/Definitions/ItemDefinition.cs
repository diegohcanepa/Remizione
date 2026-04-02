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
            AreaRange = element.GetInt32("areaRange", 0);
            if (AreaRange < 0)
                AreaRange = 0;

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

            // FaithCost
            FaithCost = element.GetInt32("faithCost", 0);
            if (FaithCost < 0)
                FaithCost = 0;

            // HP
            DiceExpression? hp = element.GetObject("hp", value => new DiceExpression(value));

            // ImpactWord
            var impactWord = element.GetEnum("impactWord", ImpactWordName.None);

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
            Sound = element.GetObject("sound", Sound.Get);

            this.Description = Localization.GetItemDescription(this);
            this.DisplayName = Localization.GetItemName(this);
            this.Image = Atlases.UI.FindImage(Name);

            this.Price = Quality switch
            {
                0 or 1 => 5,  // Items básicos o consumibles
                2 or 3 => 10, // Herramientas y gadgets de nivel medio
                4 or 5 => 15, // Items poderosos o de alta calidad
                _ => 5
            };

            Definitions.Add(this);
        }

        #endregion

        // AreaRange
        public int AreaRange { get; }

        // Category
        public ItemCategory Category { get; }

        // ConsumptionInterval
        public int ConsumptionInterval { get; }

        // ConsumptionType
        public ConsumptionType ConsumptionType { get; }

        // Definitions
        public static ItemDefinitionContainer Definitions { get; } = new(element => new ItemDefinition(element));

        // Description
        public string Description { get; }

        // DisplayName
        public string DisplayName { get; }

        // Durability
        public Ratio Durability { get; }

        // DurabilityCost
        public float DurabilityCost { get; }

        // ExecutionDelay
        public int ExecutionDelay { get; }

        // FaithCost
        public int FaithCost { get; }

        // Image
        public AtlasImage? Image { get; }

        // IsStackable
        public bool IsStackable { get; }

        // PickupSound
        public Sound PickupSound { get; }

        // Price
        public int Price { get; }

        // Quality
        public int Quality { get; }

        // Realm
        public Realm Realm { get; }

        // SkillChance
        public int SkillChance { get; }

        // Sound
        public Sound? Sound { get; }
    }
}
