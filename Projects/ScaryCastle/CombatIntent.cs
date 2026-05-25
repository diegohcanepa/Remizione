using Engendro;
using Engendro.Audio;
using System;
using System.Collections.ObjectModel;
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
                AnimationName = $"Use{Name}";

            // AreaOfEffect
            AreaOfEffect = element.GetInt32("areaOfEffect", 0);
            if (AreaOfEffect < 0)
                AreaOfEffect = 0;

            // Category
            this.Category = element.GetEnum("category", CombatIntentCategory.Basic);

            // Contact
            this.Contact = string.Equals(Name, nameof(Contact), StringComparison.OrdinalIgnoreCase);

            // InPlaceEffectType
            InPlaceEffectType = element.GetEnum("inPlaceEffectType", InPlaceEffectType.None);

            // Range
            this.Range = element.GetInt32("range", 5);

            // Sound
            this.Sound = element.GetObject("sound", Sound.Get);

            // UsageScope
            UsageScope = element.GetEnum("usageScope", ItemUsageScope.Close);

            if (UsageScope == ItemUsageScope.Projectile && Projectile == null)
                RaiseValidationError(this, "Items with Projectile usage scope must have a Projectile defined.", nameof(UsageScope));
        }

        #region IGameAction interface

        void IGameAction.Consume()
        {
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

        // Projectile
        public ProjectileDescriptor? Projectile { get; }

        // Range
        public int Range { get; }

        // Sound
        public Sound? Sound { get; }

        // UsageScope
        public ItemUsageScope UsageScope { get; }
    }
}