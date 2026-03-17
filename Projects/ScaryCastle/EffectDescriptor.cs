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
            Amount = element.GetObject("amount", v => new DiceExpression(v));
            Chance = element.GetFloat("chance", 1);
            DamageType = element.GetEnum<DamageType>("damageType", DamageType.Physical);
            EffectType = element.GetEnum("effectType", EffectType.None);
            Factor = element.GetFloat("factor", 1);
            ImpactWord = element.GetEnum("impactWord", ImpactWordName.None);
            Knockback = element.GetVector2("knockback", Vector2.Zero);
            Sound = element.GetObject("sound", Sound.Get);
            Target = element.GetEnum("target", EffectTarget.Target);
        }

        #endregion

        #region Static members

        // Apply
        public static void Apply(GameThing source, GameThing target)
        {
            if (source is IThingDefinition t && t.Definition != null)
                Apply(t.Definition.EffectDescriptors, source, target);
        }

        // Apply
        public static void Apply(IList<EffectDescriptor> effects, GameThing source, GameThing target)
        {
            if (effects.Count == 0)
                return;

            foreach (var effect in effects)
            {
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
                    // Heal
                    case EffectType.Heal:
                        realTarget.Heal(amount);
                        break;

                    // Damage
                    case EffectType.Damage:
                        realTarget.TakeDamage(source, effect.DamageType, amount, effect.ImpactWord, effect.Knockback);
                        break;

                    // Death
                    case EffectType.Death:
                        realTarget.TakeDamage(source, effect.DamageType, int.MaxValue, effect.ImpactWord, effect.Knockback);
                        break;
                }
            }
        }

        #endregion

        // Amount
        public DiceExpression? Amount { get; }

        // Chance
        public Ratio Chance { get; }

        // DamageType
        public DamageType DamageType { get; }

        // EffectType
        public EffectType EffectType { get; }

        // Factor
        public float Factor { get; }

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
