using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// EffectDescriptor
    /// </summary>
    public sealed class EffectDescriptor
    {
        // Constructor privado
        public EffectDescriptor(JsonElement element)
        {
            Amount = element.GetObject("amount", v => new DiceExpression(v));
            AttackType = element.GetEnum("attackType", AttackType.None);
            Chance = element.GetFloat("chance", 1);
            DamageType = element.GetEnum("damageType", DamageType.None);
            EffectType = element.GetEnum("effectType", EffectType.None);
            ImpactWord = element.GetEnum("impactWord", ImpactWordName.None);
            Knockback = element.GetVector2("knockback");
            Sound = element.GetObject("sound", Sound.Get);
            Target = element.GetEnum("target", EffectTarget.Target);

            if (EffectType == EffectType.Damage && DamageType == DamageType.None)
                throw new InvalidOperationException("Damage effects must have a valid damage type.");
        }

        // Amount
        public DiceExpression? Amount { get; }

        // Apply
        public static void Apply(IList<EffectDescriptor> effects, GameThing source, GameThing target, AttackType attackType)
        {
            if (effects.Count == 0)
                return;

            foreach (var effect in effects)
            {
                if (!effect.Chance.Roll())
                    continue;

                if (effect.AttackType != AttackType.None && effect.AttackType != attackType)
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
                    // Heal
                    case EffectType.Heal:
                        realTarget.HP += amount;
                        break;

                    // Damage
                    case EffectType.Damage:
                        realTarget.TakeDamage(source, effect.AttackType, effect.DamageType, amount, effect.ImpactWord, effect.Knockback);
                        break;
                }
            }
        }

        // AttackType
        public AttackType AttackType { get; }

        // Chance
        public Ratio Chance { get; }

        // DamageType
        public DamageType DamageType { get; }

        // EffectType
        public EffectType EffectType { get; }

        // ImpactWord
        public ImpactWordName ImpactWord { get; }

        // Knockback
        public Vector2 Knockback { get; }

        // Sound
        public Sound? Sound { get; }

        // Target
        public EffectTarget Target { get; }
    }
}
