using Engendro;
using Engendro.Audio;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// CombatIntent
    /// </summary>
    public sealed class CombatIntent : Definition, IAction
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

            // EnergyCost
            EnergyCost = element.GetInt32("energyCost", 0);
            if (EnergyCost < 0)
                EnergyCost = 0;

            // InPlaceEffectType
            InPlaceEffectType = element.GetEnum("inPlaceEffectType", InPlaceEffectType.None);

            // SoundStart
            this.SoundStart = element.GetObject("soundStart", Sound.Get);

            // SoundTrigger
            this.SoundTrigger = element.GetObject("soundTrigger", Sound.Get);

            // ActionKind
            ActionKind = element.GetEnum("actionKind", ActionKind.Proximity);

            if (ActionKind == ActionKind.Projectile && Projectile == null)
                RaiseValidationError(this, "Items with Projectile action must have a Projectile defined.", nameof(ActionKind));
        }

        #region IAction interface

        void IAction.Consume(Actor actor)
        {
            actor.Energy -= EnergyCost;
        }

        #endregion

        // ActionKind
        public ActionKind ActionKind { get; }

        // AnimationName
        public string AnimationName { get; }

        // AreaOfEffect
        public int AreaOfEffect { get; }

        // Category
        public CombatIntentCategory Category { get; }

        // InPlaceEffectType
        public InPlaceEffectType InPlaceEffectType { get; }

        // EnergyCost
        public int EnergyCost { get; }

        // Projectile
        public ProjectileDescriptor? Projectile { get; }

        // Range

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }
    }
}