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
            this.ComicText = element.GetEnum("comicText", ComicTextKind.None);
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

                    // CoinLoss
                    case EffectType.CoinLoss:
                        source.Session.Coins -= amount;
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
                        targetActor?.Energy += amount;
                        break;

                    // ExtraEnergy
                    case EffectType.ExtraEnergy:
                        targetActor?.MaxEnergy += amount;
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

                    // HPGain
                    case EffectType.HPGain:
                        realTarget?.HP += amount;
                        targetActor?.StatusManager.GetStatus(ScaryCastle.StatusType.Poison).Value = 0;
                        break;

                    // HPLoss
                    case EffectType.HPLoss:
                        realTarget?.HP -= amount;
                        break;

                    // Status
                    case EffectType.Status:
                        if (effect.StatusType is StatusType statusType && targetActor != null)
                        {
                            var status = targetActor.StatusManager.GetStatus(statusType);
                            var wasZero = status.Value == 0;
                            status.Value += amount;
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

                // HPGain / HPLoss
                if (effect.EffectType is EffectType.HPGain or EffectType.HPLoss)
                {
                    if (effect.Amount != null)
                        value = GetTemplate(effect.EffectType).Replace("{amount}", effect.Amount.ToString());
                }

                // Damage
                if (effect.EffectType is EffectType.Damage)
                {
                    if (effect.Amount != null)
                    {
                        value = GetTemplate(effect.EffectType).Replace("{amount}", effect.Amount.ToString());
                        value += $" ({Localization.GetValue(effect.DamageType)})";
                    }
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
