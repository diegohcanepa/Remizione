using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// EffectDescriptor
    /// </summary>
    public sealed class EffectDescriptor
    {
        private static readonly Vector2 KnockbackLow = new(15, 5);
        private static readonly Vector2 KnockbackMedium = new(25, 5);
        private static readonly Vector2 KnockbackHigh = new(35, 5);

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
            this.Context = element.GetEnum("context", EffectContext.Contact);
            this.DamageType = element.GetEnum("damageType", DamageType.Physical);
            this.EffectType = element.GetEnum("effectType", EffectType.None);
            this.Knockback = element.GetEnum("knockback", KnockbackIntensity.Low);
            this.Sound = element.GetObject("sound", Sound.Get);
            this.StatusType = element.GetEnum<StatusType>("statusType");
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

                // Out of context
                if (effect.Context != context)
                    continue;

                // Roll dice
                if (!effect.Chance.Roll())
                    continue;

                var realTarget = effect.Target == EffectTarget.Self ? source : target;
                var targetActor = realTarget as Actor;

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

                    // Damage
                    case EffectType.Damage:
                        realTarget?.TakeDamage(source, effect.DamageType, amount, effect.GetKnockbackForce());
                        break;

                    // Death
                    case EffectType.Death:
                        realTarget?.TakeDamage(source, effect.DamageType, int.MaxValue, effect.GetKnockbackForce());
                        break;

                    // EnergyGain
                    case EffectType.EnergyGain:
                        targetActor?.Energy += amount;
                        break;

                    // EnergyLoss
                    case EffectType.EnergyLoss:
                        if (targetActor != null)
                            targetActor.Energy -= amount;
                        break;

                    // EnergyRestore
                    case EffectType.EnergyRestore:
                        targetActor?.Recharge();
                        break;

                    // HPGain
                    case EffectType.HPGain:
                        realTarget?.HP += amount;
                        targetActor?.StatusManager.Discard(Remizione.StatusType.Poison);
                        break;

                    // HPLoss
                    case EffectType.HPLoss:
                        realTarget?.HP -= amount;
                        break;

                    // HPRestore
                    case EffectType.HPRestore:
                        realTarget?.Reheal();
                        break;

                    // MaxEnergyGain
                    case EffectType.MaxEnergyGain:
                        targetActor?.MaxEnergy += amount;
                        break;

                    // MaxEnergyLoss
                    case EffectType.MaxEnergyLoss:
                        if (targetActor?.MaxEnergy > 3)
                            targetActor.MaxEnergy -= amount;
                        break;

                    // MaxHPGain
                    case EffectType.MaxHPGain:
                        if (realTarget != null)
                            realTarget.MaxHP += amount;
                        break;

                    // MaxHPLoss
                    case EffectType.MaxHPLoss:
                        realTarget?.MaxHP -= amount;
                        break;

                    // Status
                    case EffectType.Status:
                        if (effect.StatusType is StatusType statusType && targetActor != null && !targetActor.IsDead)
                        {
                            var status = targetActor.StatusManager.Apply(statusType, amount);
                            targetActor.ShowStatusReaction(status, true);
                        }
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

        // GetDescription
        public static string GetDescription(IList<EffectDescriptor> effects)
        {
            static string GetTemplate(EffectType effectType)
            {
                return TextRepository.GetValue($"EffectTemplate.{effectType}");
            }

            var values = new List<string>();

            foreach (var effect in effects)
            {
                var value = string.Empty;

                // Damage
                if (effect.EffectType is EffectType.Damage)
                {
                    if (effect.Amount != null)
                    {
                        value = GetTemplate(effect.EffectType).Replace("{amount}", effect.Amount.ToString());
                        value += $" ({Localization.GetValue(effect.DamageType)})";
                    }
                }

                // HPGain / HPLoss
                if (effect.EffectType is EffectType.HPGain or EffectType.HPLoss)
                {
                    if (effect.Amount != null)
                        value += GetTemplate(effect.EffectType).Replace("{amount}", effect.Amount.ToString());
                }

                // EnergyRestore
                if (effect.EffectType is EffectType.EnergyRestore)
                {
                    value += GetTemplate(effect.EffectType);
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (effect.Chance > 0 && effect.Chance < 1)
                        value += $" [{effect.Chance * 100}%]";

                    values.Add(value);
                }
            }

            return string.Join("/", values);
        }

        #endregion

        // Amount
        public DiceExpression? Amount { get; }

        // Chance
        public Ratio Chance { get; }

        // Context
        public EffectContext Context { get; }

        // DamageType
        public DamageType DamageType { get; }

        // EffectType
        public EffectType EffectType { get; }

        // GetKnockbackForce
        public Vector2 GetKnockbackForce()
        {
            return Knockback switch
            {
                KnockbackIntensity.None => Vector2.Zero,
                KnockbackIntensity.Low => KnockbackLow,
                KnockbackIntensity.Medium => KnockbackMedium,
                KnockbackIntensity.High => KnockbackHigh,
                _ => throw new System.NotImplementedException(),
            };
        }

        // Knockback
        public KnockbackIntensity Knockback { get; }

        // Sound
        public Sound? Sound { get; }

        // StatusType
        public StatusType? StatusType { get; }

        // Target
        public EffectTarget Target { get; }
    }
}
