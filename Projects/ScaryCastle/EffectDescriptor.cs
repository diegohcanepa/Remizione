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
        public EffectDescriptor()
        {
        }

        // Constructor
        public EffectDescriptor(JsonElement element)
        {
            this.Amount = element.GetObject("amount", v => new DiceExpression(v));
            this.Chance = element.GetFloat("chance", 1);
            this.ComicText = element.GetEnum("comicText", ComicTextKind.None);
            this.ConditionType = element.GetEnum("conditionType", ConditionType.None);
            this.Context = element.GetEnum("context", EffectContext.Contact);
            this.DamageType = element.GetEnum("damageType", DamageType.Physical);
            this.EffectType = element.GetEnum("effectType", EffectType.None);
            this.Knockback = element.GetEnum("knockback", KnockbackIntensity.Low);
            this.Sound = element.GetObject("sound", Sound.Get);
            this.Target = element.GetEnum("target", EffectTarget.Target);
        }

        #endregion

        #region Static members

        // Apply
        public static void Apply(IList<EffectDescriptor> effects, GameThing source, GameThing? target, EffectContext context)
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

                    // BronzeKey
                    case EffectType.BronzeKey:
                        source.Session.BronzeKeys += amount;
                        break;

                    // ComicText
                    case EffectType.ComicText:
                        if (effect.ComicText != ComicTextKind.None)
                            realTarget?.ShowComicText(effect.ComicText);
                        break;

                    // Coin
                    case EffectType.Coin:
                        source.Session.Coins += amount;
                        break;

                    // Condition
                    case EffectType.Condition:
                        (realTarget as Actor)?.ApplyCondition(effect.ConditionType, amount, effect.ComicText);
                        break;

                    // Damage
                    case EffectType.Damage:
                        realTarget?.TakeDamage(source, effect.DamageType, amount, effect.ComicText, effect.GetKnockbackForce());
                        break;

                    // Death
                    case EffectType.Death:
                        realTarget?.TakeDamage(source, effect.DamageType, int.MaxValue, effect.ComicText, effect.GetKnockbackForce());
                        break;

                    // Energy
                    case EffectType.Energy:
                        (realTarget as Actor)?.Energy += amount;
                        break;

                    // ExtraEnergy
                    case EffectType.ExtraEnergy:
                        (realTarget as Actor)?.MaxEnergy += amount;
                        source.Session.TextHUD.Message.Show(MessageKind.ExtraEnergy);
                        break;

                    // ExtraHeart
                    case EffectType.ExtraHeart:
                        realTarget?.MaxHP += amount;
                        source.Session.TextHUD.Message.Show(MessageKind.ExtraHeart);
                        break;

                    // GoldenKey
                    case EffectType.GoldenKey:
                        source.Session.GoldenKeys += amount;
                        break;

                    // Heal
                    case EffectType.Heal:
                        realTarget?.HP += amount;
                        if (realTarget is Actor actor && actor.Condition == ConditionType.Poison)
                            actor.ConditionAmount -= amount;
                        break;
                }
            }
        }

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
        public DiceExpression? Amount { get; init; }

        // Chance
        public Ratio Chance { get; init; }

        // ConditionType
        public ConditionType ConditionType { get; init; }

        // ComicText
        public ComicTextKind ComicText { get; init; }

        // Context
        public EffectContext Context { get; init; }

        // DamageType
        public DamageType DamageType { get; init; }

        // EffectType
        public EffectType EffectType { get; init; }

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
        public KnockbackIntensity Knockback { get; init; }

        // Sound
        public Sound? Sound { get; init; }

        // Target
        public EffectTarget Target { get; init; }
    }
}
