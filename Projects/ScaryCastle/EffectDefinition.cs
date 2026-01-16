using Engendro;
using Engendro.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// EffectDefinition
    /// </summary>
    public sealed class EffectDefinition
    {
        // Constructor privado
        public EffectDefinition(JsonElement element)
        {
            Amount = element.GetObject("amount", v => new DiceExpression(v));
            DamageType = element.GetEnum("damageType", DamageType.None);
            EffectType = element.GetEnum("effectType", EffectType.None);
            Sound = element.GetObject("sound", Sound.Get);
            Target = element.GetEnum("target", EffectTarget.Target);
        }

        // Amount
        public DiceExpression? Amount { get; }

        // DamageType
        public DamageType DamageType { get; }

        // EffectType
        public EffectType EffectType { get; }

        // Sound
        public Sound? Sound { get; }

        // Target
        public EffectTarget Target { get; }
    }
}
