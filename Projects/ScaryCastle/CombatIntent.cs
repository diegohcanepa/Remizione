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

            // GooCost
            GooCost = element.GetInt32("gooCost", 0);
            if (GooCost < 0)
                GooCost = 0;

            // InPlaceEffectType
            InPlaceEffectType = element.GetEnum("inPlaceEffectType", InPlaceEffectType.None);

            // Range
            this.Range = element.GetInt32("range", 5);

            // SoundStart
            this.SoundStart = element.GetObject("soundStart", Sound.Get);

            // SoundTrigger
            this.SoundTrigger = element.GetObject("soundTrigger", Sound.Get);

            // UsageScope
            UsageScope = element.GetEnum("usageScope", ItemUsageScope.Close);

            if (UsageScope == ItemUsageScope.Projectile && Projectile == null)
                RaiseValidationError(this, "Items with Projectile usage scope must have a Projectile defined.", nameof(UsageScope));
        }

        #region IGameAction interface

        void IGameAction.Consume(Actor actor)
        {
            actor.Goo -= GooCost;
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

        // GooCost
        public int GooCost { get; }

        // Projectile
        public ProjectileDescriptor? Projectile { get; }

        // Range
        public int Range { get; }

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }

        // UsageScope
        public ItemUsageScope UsageScope { get; }
    }
}