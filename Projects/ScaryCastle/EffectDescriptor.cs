using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// EffectDescriptor
    /// </summary>
    public sealed class EffectDescriptor
    {
        #region Constructor

        // Constructor
        public EffectDescriptor(JsonElement element)
        {
            this.Amount = element.GetObject("amount", v => new DiceExpression(v));
            this.Chance = element.GetFloat("chance", 1);
            this.ComicText = element.GetEnum("comicText", ComicTextKind.None);
            this.Condition = element.GetEnum("conditionType", ConditionType.None);
            this.Context = element.GetEnum("context", EffectContext.Contact);
            this.DamageType = element.GetEnum("damageType", DamageType.Physical);
            this.EffectType = element.GetEnum("effectType", EffectType.None);
            this.Modifier = element.GetFloat("modifier", 0);
            this.Knockback = element.GetEnum("knockback", KnockbackIntensity.Low);
            this.Sound = element.GetObject("sound", Sound.Get);
            this.Target = element.GetEnum("target", EffectTarget.Target);
        }

        #endregion

        #region Static members

        // Apply
        public static void Apply(IList<EffectDescriptor> effects, GameThing source, GameThing target, EffectContext context)
        {
            if (effects.Count == 0)
                return;

            for (var i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];

                if (effect.Context != context)
                    continue;

                if (!effect.Chance.Roll())
                    continue;

                var realTarget = effect.Target == EffectTarget.Self ? source : target;
                var actor = realTarget as Actor;

                // Play sound
                if (effect.Sound != null)
                    source.PlaySound(effect.Sound);

                // Calculate amount
                int amount = effect.Amount == null ? 0 : effect.Amount.Roll();

                // Apply logic to target
                switch (effect.EffectType)
                {
                    // None / Luck
                    case EffectType.None:
                        break;

                    // ComicText
                    case EffectType.ComicText:
                        // ComicText si hubo daño real
                        if (effect.ComicText != ComicTextKind.None)
                            realTarget.ShowComicText(effect.ComicText);
                        break;

                    // Condition
                    case EffectType.Condition:
                        realTarget.ApplyCondition(effect.Condition, amount, effect.ComicText);
                        break;

                    // Damage
                    case EffectType.Damage:
                        realTarget.TakeDamage(source, effect.DamageType, amount, effect.ComicText, effect.GetKnockbackForce());
                        break;

                    // Death
                    case EffectType.Death:
                        realTarget.TakeDamage(source, effect.DamageType, int.MaxValue, effect.ComicText, effect.GetKnockbackForce());
                        break;

                    // Goo
                    case EffectType.Goo:
                        actor?.Goo += amount;
                        break;

                    // Heal
                    case EffectType.Heal:
                        realTarget.HP += amount;
                        break;
                }
            }
        }

        // ConditionType
        public ConditionType Condition { get; }

        // Contains
        public static bool Contains(IList<EffectDescriptor> effects, EffectContext context)
        {
            for (var i = 0; i < effects.Count; i++)
            {
                if (effects[i].Context == context)
                    return true;
            }

            return false;
        }

        #endregion

        // Amount
        public DiceExpression? Amount { get; }

        // Chance
        public Ratio Chance { get; }

        // ComicText
        public ComicTextKind ComicText { get; }

        // Context
        public EffectContext Context { get; }

        // DamageType
        public DamageType DamageType { get; }

        // EffectType
        public EffectType EffectType { get; }

        // GetKnockbackForce
        public Vector2 GetKnockbackForce()
        {
            if (Knockback == KnockbackIntensity.High)
                return new(35, 5);

            else if (Knockback == KnockbackIntensity.Medium)
                return new(25, 5);

            else
                return new(15, 5);
        }

        // Knockback
        public KnockbackIntensity Knockback { get; }

        // Modifier
        public float Modifier { get; }

        // Sound
        public Sound? Sound { get; }

        // Target
        public EffectTarget Target { get; }
    }
}
