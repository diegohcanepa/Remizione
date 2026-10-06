using Engendro;
using Engendro.Audio;
using System;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// CombatIntent
    /// </summary>
    public sealed class CombatIntent : Definition, IAction
    {
        // Constructor
        public CombatIntent(JsonElement element)
            : base(element, NameValidationRule.AllowDuplicates)
        {
            // ActionKind
            ActionKind = element.GetEnum("actionKind", ActionKind.Proximity);

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

            // HPCost
            HPCost = Math.Max(0, element.GetInt32("hpCost", 0));

            // ImpactEffectType
            ImpactEffectType = element.GetEnum("impactEffectType", ImpactEffectType.None);

            // IsCharge
            IsCharge = element.GetBool("isCharge", false);

            // MinRange
            MinRange = element.GetInt32("minRange", 0);
            if (MinRange < 0)
                RaiseValidationError(this, "Property 'minRange' cannot be negative.", nameof(MinRange));

            // MaxRange
            if (element.GetInt32("maxRange") is not int maxRange)
                RaiseValidationError(this, "Property 'maxRange' is mandatory in JSON definition.", nameof(MaxRange));
            else
                MaxRange = maxRange;

            if (MinRange > MaxRange)
                RaiseValidationError(this, $"Minimum range ({MinRange}) exceeds maximum range ({MaxRange}).", nameof(MaxRange));

            // MissChance
            MissChance = Math.Max(0, element.GetFloat("missChance", 0));

            // SoundStart
            this.SoundStart = element.GetObject("soundStart", Sound.Get);

            // SoundTrigger
            this.SoundTrigger = element.GetObject("soundTrigger", Sound.Get);

            // TelegraphAnimationName
            TelegraphAnimationName = element.GetString("telegraphAnimationName");

            // TelegraphKind
            TelegraphKind = element.GetEnum("telegraphKind", AttackTelegraphKind.None);
        }

        #region IAction interface

        // Consume
        void IAction.Consume(Actor actor)
        {
            actor.ApplyAction(this);
        }

        // ProjectileImageName
        string IAction.ProjectileImageName => string.Empty;

        #endregion

        // ActionKind
        public ActionKind ActionKind { get; }

        // AnimationName
        public string AnimationName { get; }

        // AreaOfEffect
        public int AreaOfEffect { get; }

        // Category
        public CombatIntentCategory Category { get; }

        // HPCost
        public int HPCost { get; }

        // ImpactEffectType
        public ImpactEffectType ImpactEffectType { get; }

        // IsCharge
        public bool IsCharge { get; }

        // MaxRange
        public int MaxRange { get; }

        // MinRange
        public int MinRange { get; }

        // MissChance
        public Ratio MissChance { get; }

        // SoundStart
        public Sound? SoundStart { get; }

        // SoundTrigger
        public Sound? SoundTrigger { get; }

        // TelegraphAnimationName
        public string TelegraphAnimationName { get; }

        // TelegraphKind
        public AttackTelegraphKind TelegraphKind { get; }
    }
}