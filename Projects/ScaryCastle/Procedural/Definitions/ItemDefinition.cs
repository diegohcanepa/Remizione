using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
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
            // ActionKind
            ActionKind = element.GetEnum("actionKind", ActionKind.Script);

            // AnimationName
            AnimationName = element.GetString("animationName");
            if (string.IsNullOrWhiteSpace(AnimationName))
                AnimationName = $"Use{Name}";

            // AreaOfEffect
            AreaOfEffect = element.GetInt32("areaOfEffect", 0);
            if (AreaOfEffect < 0)
                AreaOfEffect = 0;

            // Category
            Category = element.GetEnum("category", ItemCategory.Misc);

            // ConsumeVerb
            ConsumeVerb = element.GetEnum("consumeVerb", LogVerb.None);

            // DeselectOnUse
            DeselectOnUse = element.GetBool("deselectOnUse", false);

            // EnergyCost
            EnergyCost = element.GetInt32("energyCost", 0);
            if (EnergyCost < 0)
                EnergyCost = 0;

            // InitialAmount
            InitialAmount = int.Clamp(element.GetInt32("initialAmount", 1), 1, GameSettings.MaxItemAmount);

            // InPlaceEffectType
            InPlaceEffectType = element.GetEnum("inPlaceEffectType", InPlaceEffectType.None);

            // IsDepletable
            IsDepletable = element.GetBool("isDepletable", true);

            // IsSkill
            IsSkill = element.GetBool("isSkill", false);

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

            // SoundStart
            this.SoundStart = element.GetObject("soundStart", Sound.Get);

            // SoundTrigger
            this.SoundTrigger = element.GetObject("soundTrigger", Sound.Get);

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

            if (IsSkill)
            {
                IsDepletable = false;
                IsStackable = false;
            }

            if (ActionKind == ActionKind.Projectile && Projectile == null)
                RaiseValidationError(this, "Items with Projectile usage mode must have a Projectile defined.", nameof(ActionKind));

            Definitions.Add(this);
        }

        #endregion

        // ActionKind
        public ActionKind ActionKind { get; }

        // AnimationName
        public string AnimationName { get; }

        // AreaOfEffect
        public int AreaOfEffect { get; }

        // Category
        public ItemCategory Category { get; }

        // ConsumeVerb
        public LogVerb ConsumeVerb { get; }

        // Definitions
        public static ItemDefinitionContainer Definitions { get; } = new(element => new ItemDefinition(element));

        // Description
        public string Description { get; }

        // DeselectOnUse
        public bool DeselectOnUse { get; }

        // DisplayName
        public string DisplayName { get; }

        // EnergyCost
        public int EnergyCost { get; }

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

        // IsSkill
        public bool IsSkill { get; }

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

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }
    }
}
