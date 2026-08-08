using Engendro;
using Engendro.Audio;
using System;
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

            // MinRange
            MinRange = element.GetInt32("minRange", 0);

            // MaxRange
            MaxRange = element.GetInt32("maxRange", int.MaxValue);

            if (MinRange > MaxRange)
                RaiseValidationError(this, $"Minimum range exceeds the maximum range.");

            // MissChance
            MissChance = Math.Max(0, element.GetFloat("missChance", 0));

            // RequiresStamina
            RequiresStamina = element.GetBool("requiresStamina", false);

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

        // Consume
        void IAction.Consume(Actor actor)
        {
            actor.Energy -= EnergyCost;
            actor.Stamina--;
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

        // MaxRange
        public int MaxRange { get; }

        // MinRange
        public int MinRange { get; }

        // MissChance
        public Ratio MissChance { get; }

        // Projectile
        public ProjectileDescriptor? Projectile { get; }

        // RequiresStamina
        public bool RequiresStamina { get; }

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }
    }
}