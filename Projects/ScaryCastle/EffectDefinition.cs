using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
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
        private static readonly Dictionary<string, EffectDefinition> effects = [];
        private static bool loaded;

        // Constructor
        private EffectDefinition(JsonElement element)
        {
            // Name
            this.Name = element.GetProperty("name").GetString() ?? throw new InvalidDataException("Name not found.");

            CodeContract.ValidName(this.Name, string.Empty);

            Utils.AssertName(Name, this);

            // Damage
            if (element.TryGetProperty("damage", out JsonElement damageElement) && damageElement.GetString() is string damageValue)
                Damage = new(damageValue);

            // DamageType
            if (element.TryGetProperty("damageType", out JsonElement damageTypeElement) && damageTypeElement.GetString() is string damageTypeValue)
                DamageType = Enum.Parse<DamageType>(damageTypeValue);

            // HP
            if (element.TryGetProperty("hp", out JsonElement hpElement) && hpElement.GetString() is string hpValue)
                HP = new(hpValue);

            // ImpactWord
            if (element.TryGetProperty("impactWord", out JsonElement impactWordElement) && impactWordElement.GetString() is string impactWordValue)
                ImpactWord = Enum.Parse<ImpactWordName>(impactWordValue);

            // Knockback
            if (element.TryGetProperty("knockback", out JsonElement knockbackElement) && knockbackElement.GetString() is string knockbackValue)
                Knockback = DataConverter.ToVector2(knockbackValue);

            // LuckBonus
            if (element.TryGetProperty("luckBonus", out JsonElement luckBonusElement))
                LuckBonus = luckBonusElement.GetSingle();

            // Sound
            if (element.TryGetProperty("sound", out JsonElement soundElement) && soundElement.GetString() is string soundValue)
                Sound = Sound.Get(soundValue);

            effects.Add(Name, this);
        }

        // Constructor
        public EffectDefinition(string name)
        {
            this.Name = name;
        }

        #region Static members

        // All
        public static IEnumerable<EffectDefinition> All => effects.Values;

        // Find
        public static EffectDefinition? Find(string name)
        {
            return effects.TryGetValue(name, out var result) ? result : null;
        }

        // Get
        public static EffectDefinition Get(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"{nameof(EffectDefinition)} '{name}' not found.");
        }

        // Load
        public static void Load(string fileName)
        {
            if (loaded)
                throw new InvalidOperationException("Data is already loaded.");

            Utils.LoadJsonData(fileName, element => new EffectDefinition(element));

            loaded = true;
        }

        #endregion

        // ApplyDamage
        public bool ApplyDamage(GameThing attacker, GameThing target)
        {
            if (Damage == null)
                return false;

            target.TakeDamage(attacker, this);

            return true;
        }

        // ApplyHP
        public bool ApplyHP(GameThing target)
        {
            if (HP == null)
                return false;

            target.HP += HP.Roll();

            return true;
        }

        // Damage
        public DiceExpression? Damage { get; init; }

        // DamageType
        public DamageType DamageType { get; init; }

        // HP
        public DiceExpression? HP { get; init; }

        // ImpactWord
        public ImpactWordName ImpactWord { get; init; }

        // Knockback
        public Vector2 Knockback { get; init; }

        // LuckBonus
        public Ratio LuckBonus { get; init; }

        // Name
        public string Name { get; }

        // Sound
        public Sound? Sound { get; init; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
