using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ItemDefinition
    /// </summary>
    public sealed class ItemDefinition : Definition, IGameAction
    {
        #region Constructor

        // Constructor
        private ItemDefinition(JsonElement element)
            : base(element)
        {
            // AnimationName
            AnimationName = element.GetString("animationName");
            if (string.IsNullOrWhiteSpace(AnimationName))
                AnimationName = $"Use{Name}";

            // AreaRange
            AreaRange = element.GetInt32("areaRange", 0);
            if (AreaRange < 0)
                AreaRange = 0;

            // Category
            Category = element.GetEnum("category", ItemCategory.Misc);

            // DeselectOnUse
            DeselectOnUse = element.GetBool("deselectOnUse", false);

            // GooCost
            GooCost = element.GetInt32("gooCost", 0);
            if (GooCost < 0)
                GooCost = 0;

            // InitialAmount
            InitialAmount = element.GetInt32("initialAmount", 1);
            if (InitialAmount < 1)
                InitialAmount = 1;

            // InPlaceEffectType
            InPlaceEffectType = element.GetEnum("inPlaceEffectType", InPlaceEffectType.None);

            // IsDepletable
            IsDepletable = element.GetBool("isDepletable", true);

            // IsStackable
            IsStackable = element.GetBool("isStackable", false);

            // LightColor
            LightColor = element.GetColor("lightColor");

            // LightModifier
            LightModifier = element.GetFloat("lightModifier", 0);
            if (LightModifier < 0)
                RaiseValidationError(this, "Light modifier must be equal or greater that zero.", nameof(LightModifier));

            // LuckModifier
            LuckModifier = element.GetFloat("luckModifier", 0);

            // PickupSound
            PickupSound = element.GetObject("pickupSound", Sound.Get) ?? Sound.Get(SoundNames.PickupGeneric);

            // Projectile
            if (element.TryGetProperty("projectile", out JsonElement projectileElement) && projectileElement.ValueKind == JsonValueKind.Object)
                Projectile = new ProjectileDescriptor(projectileElement);

            // Quality
            Quality = element.GetInt32("quality", 0);

            // Realm
            Realm = element.GetEnum("realm", Realm.Earthly);

            // SkillChance
            SkillChance = element.GetInt32("skillChance", 0);

            // Sound
            Sound = element.GetObject("sound", Sound.Get);

            // UsageScope
            UsageScope = element.GetEnum("usageScope", ItemUsageScope.Close);

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

            IsPassive = LightModifier != 0 || LuckModifier != 0;

            if (UsageScope == ItemUsageScope.Projectile && Projectile == null)
                RaiseValidationError(this, "Items with Projectile usage scope must have a Projectile defined.", nameof(UsageScope));

            Definitions.Add(this);
        }

        #endregion

        // AnimationName
        public string AnimationName { get; }

        // AreaRange
        public int AreaRange { get; }

        // Category
        public ItemCategory Category { get; }

        // Definitions
        public static ItemDefinitionContainer Definitions { get; } = new(element => new ItemDefinition(element));

        // Description
        public string Description { get; }

        // DeselectOnUse
        public bool DeselectOnUse { get; }

        // DisplayName
        public string DisplayName { get; }

        // GooCost
        public int GooCost { get; }

        // Image
        public AtlasImage? Image { get; }

        // InitialAmount
        public int InitialAmount { get; }

        // InPlaceEffectType
        public InPlaceEffectType InPlaceEffectType { get; }

        // IsDepletable
        public bool IsDepletable { get; }

        // IsPassive
        public bool IsPassive { get; }

        // IsStackable
        public bool IsStackable { get; }

        // LightColor
        public Color? LightColor { get; }

        // LightModifier
        public float LightModifier { get; }

        // LuckModifier
        public float LuckModifier { get; }

        // PickupSound
        public Sound PickupSound { get; }

        // Price
        public int Price { get; }

        // Projectile
        public ProjectileDescriptor? Projectile { get; }

        // Quality
        public int Quality { get; }

        // Realm
        public Realm Realm { get; }

        // SkillChance
        public int SkillChance { get; }

        // Sound
        public Sound? Sound { get; }

        // UsageScope
        public ItemUsageScope UsageScope { get; }
    }
}
