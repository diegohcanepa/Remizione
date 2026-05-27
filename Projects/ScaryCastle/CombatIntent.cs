using Engendro;
using Engendro.Audio;
using System;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CombatIntent
    /// </summary>
    public sealed class CombatIntent : Definition, IGameAction
    {
        // Constructor
        public CombatIntent(JsonElement element)
            : base(element, false)
        {
            // AnimationName
            AnimationName = element.GetString("animationName");
            if (string.IsNullOrWhiteSpace(AnimationName))
                AnimationName = Name;

            // AreaOfEffect
            AreaOfEffect = element.GetInt32("areaOfEffect", 0);
            if (AreaOfEffect < 0)
                AreaOfEffect = 0;

            // Category
            this.Category = element.GetEnum("category", CombatIntentCategory.Basic);

            // Contact
            this.Contact = string.Equals(Name, nameof(Contact), StringComparison.OrdinalIgnoreCase);

            // EnergyCost
            EnergyCost = element.GetInt32("energyCost", 0);
            if (EnergyCost < 0)
                EnergyCost = 0;

            // InPlaceEffectType
            InPlaceEffectType = element.GetEnum("inPlaceEffectType", InPlaceEffectType.None);

            // Range
            this.Range = element.GetInt32("range", 5);

            // SoundStart
            this.SoundStart = element.GetObject("soundStart", Sound.Get);

            // SoundTrigger
            this.SoundTrigger = element.GetObject("soundTrigger", Sound.Get);

            // UsageMode
            UsageMode = element.GetEnum("usageMode", ItemUsageMode.ProximityAction);

            if (UsageMode == ItemUsageMode.ProjectileAction && Projectile == null)
                RaiseValidationError(this, "Items with Projectile usage mode must have a Projectile defined.", nameof(UsageMode));
        }

        #region IGameAction interface

        void IGameAction.Consume(Actor actor)
        {
            actor.Energy -= EnergyCost;
        }

        #endregion

        // AnimationName
        public string AnimationName { get; }

        // AreaOfEffect
        public int AreaOfEffect { get; }

        // Category
        public CombatIntentCategory Category { get; }

        // Contact
        public bool Contact { get; }

        // InPlaceEffectType
        public InPlaceEffectType InPlaceEffectType { get; }

        // EnergyCost
        public int EnergyCost { get; }

        // Projectile
        public ProjectileDescriptor? Projectile { get; }

        // Range
        public int Range { get; }

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }

        // UsageMode
        public ItemUsageMode UsageMode { get; }
    }
}