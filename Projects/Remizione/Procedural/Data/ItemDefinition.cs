using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// ItemDefinition
    /// </summary>
    public sealed class ItemDefinition : Definition
    {
        #region Constructor

        // Constructor
        public ItemDefinition(JsonElement element)
            : base(element)
        {
            // ActionKind
            ActionKind = element.GetEnum("actionKind", ActionKind.ScriptOutcome);

            // AnimationName
            AnimationName = element.GetString("animationName");
            if (string.IsNullOrWhiteSpace(AnimationName))
                AnimationName = $"Use{Name}";

            // AreaOfEffect
            AreaOfEffect = element.GetInt32("areaOfEffect", 0);
            if (AreaOfEffect < 0)
                AreaOfEffect = 0;

            // AttackRange
            AttackRange = element.GetEnum("attackRange", AttackRange.None);

            // BreakBaseChance
            BreakBaseChance = element.GetFloat("breakBaseChance", 0);

            // BreakSafeUses
            BreakSafeUses = Math.Max(0, element.GetInt32("breakSafeUses", 0));

            // BreakChanceIncrement
            BreakChanceIncrement = element.GetFloat("breakChanceIncrement", 0);

            // Category
            Category = element.GetEnum("category", ItemCategory.Misc);

            // CraftWeight
            CraftWeight = element.GetFloat("craftWeight", 0);

            // HPCost
            HPCost = Math.Max(0, element.GetInt32("hpCost", 0));

            // InitialAmount
            InitialAmount = int.Clamp(element.GetInt32("initialAmount", 1), 1, GameSettings.MaxItemAmount);

            // ImpactEffectType
            ImpactEffectType = element.GetEnum("impactEffectType", ImpactEffectType.None);

            // IsDepletable
            IsDepletable = element.GetBool("isDepletable", true);

            // IsStackable
            IsStackable = element.GetBool("isStackable", false);

            // ItemKind
            ItemKind = element.GetEnum("itemKind", ItemKind.Common);

            // LightColor
            LightColor = element.GetColor("lightColor");

            // LightModifier
            LightModifier = element.GetFloat("lightModifier", 0);
            if (LightModifier < 0)
                RaiseValidationError(this, "Light modifier must be equal or greater that zero.", nameof(LightModifier));

            // LostOnDeath
            LostOnDeath = element.GetBool("lostOnDeath", false);

            // LuckModifier
            LuckModifier = element.GetFloat("luckModifier", 0);

            // PickupSound
            PickupSound = element.GetObject("pickupSound", Sound.Get) ?? Sound.Get(SoundNames.PickupGeneric);

            // ProjectileImageName
            ProjectileImageName = element.GetString("projectileImageName");

            // SkillChance
            SkillChance = element.GetInt32("skillChance", 0);

            // SoundStart
            this.SoundStart = element.GetObject("soundStart", Sound.Get);

            // SoundTrigger
            this.SoundTrigger = element.GetObject("soundTrigger", Sound.Get);

            this.Image = Atlases.UI.GetImage(Name);

            IsPassive = LightModifier != 0 || LuckModifier != 0;

            if (ActionKind == ActionKind.Projectile && string.IsNullOrWhiteSpace(ProjectileImageName))
                ProjectileImageName = Name;

            if (IsStackable)
            {
                if (ItemKind == ItemKind.Organic)
                    RaiseValidationError(this, "An organic item cannot be stackable.", nameof(IsStackable));

                if (BreakBaseChance > 0 || BreakChanceIncrement > 0)
                    RaiseValidationError(this, "A breakable item cannot be stackable.", nameof(IsStackable));
            }

            if (ItemKind == ItemKind.Key && LostOnDeath)
                RaiseValidationError(this, "Key items cannot be lost.", nameof(LostOnDeath));

            // LabelColor / OrbLightKind
            if (ItemKind == ItemKind.Key)
            {
                LabelColor = Colors.Inventory.KeyItem;
                OrbLightKind = LightKind.KeyItemOrb;
            }
            else if (ItemKind == ItemKind.Organic)
            {
                LabelColor = Colors.Inventory.OrganicItem;
                OrbLightKind = LightKind.OrganicItemOrb;
            }
            else
            {
                LabelColor = Colors.Inventory.CommonItem;
                OrbLightKind = LightKind.CommonItemOrb;
            }
            
            RefreshLocalizedValues();
        }

        #endregion

        #region Protected members

        // OnRefreshLocalizedValues
        protected override void OnRefreshLocalizedValues()
        {
            base.OnRefreshLocalizedValues();

            this.Description = TextRepository.GetValue($"Item.{Name}.Description");
            this.Label = TextRepository.GetValue($"Item.{Name}.Name");
            var effect = TextRepository.GetValue($"Item.{Name}.Effect");
            var effectDescriptors = EffectDescriptor.GetDescription(EffectDescriptors);
            var hasEffect = !string.IsNullOrWhiteSpace(effect);
            var hasEffectDescriptors = !string.IsNullOrWhiteSpace(effectDescriptors);

            if (hasEffect && hasEffectDescriptors)
            {
                this.EffectDescription = $"{effect} | {effectDescriptors}";
            }
            else if (hasEffect)
            {
                this.EffectDescription = effect;
            }
            else if (hasEffectDescriptors)
            {
                this.EffectDescription = effectDescriptors;
            }
            else
            {
                this.EffectDescription = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(Label))
                RaiseValidationError(this, "Missing localized label.");
        }

        #endregion

        // ActionKind
        public ActionKind ActionKind { get; }

        // AnimationName
        public string AnimationName { get; }

        // AreaOfEffect
        public int AreaOfEffect { get; }

        // AttackRange
        public AttackRange AttackRange { get; }

        // BreakBaseChance
        public Ratio BreakBaseChance { get; }

        // BreakSafeUses
        public int BreakSafeUses { get; }

        // BreakChanceIncrement
        public Ratio BreakChanceIncrement { get; }

        // Category
        public ItemCategory Category { get; }

        // CraftWeight
        public Ratio CraftWeight { get; }

        // Description
        public string Description { get; private set; } = string.Empty;

        // EffectDescription
        public string EffectDescription { get; private set; } = string.Empty;

        // GetLabel
        public string GetLabel(int amount)
        {
            return amount == 1 ? Label : Label + $" (x{amount})";
        }

        // HasBreakChance
        public bool HasBreakChance => BreakChanceIncrement > 0 || BreakBaseChance > 0;

        // HPCost
        public int HPCost { get; }

        // Image
        public AtlasImage Image { get; }

        // InitialAmount
        public int InitialAmount { get; }

        // ImpactEffectType
        public ImpactEffectType ImpactEffectType { get; }

        // IsDepletable
        public bool IsDepletable { get; }

        // IsPassive
        public bool IsPassive { get; }

        // IsStackable
        public bool IsStackable { get; }

        // ItemKind
        public ItemKind ItemKind { get; }

        // Label
        public string Label { get; private set; } = string.Empty;

        // LabelColor
        public Color LabelColor { get; }

        // LightColor
        public Color? LightColor { get; }

        // LightModifier
        public float LightModifier { get; }

        // LostOnDeath
        public bool LostOnDeath { get; }

        // LuckModifier
        public float LuckModifier { get; }

        // MaxStack
        public const int MaxStack = 5;

        // OrbLightKind
        public LightKind OrbLightKind { get; }

        // PickupSound
        public Sound PickupSound { get; }

        // Price
        public int Price { get; }

        // ProjectileImageName
        public string ProjectileImageName { get; }

        // SkillChance
        public int SkillChance { get; }

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }
    }
}
