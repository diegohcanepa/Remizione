using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// MetaItem
    /// </summary>
    public sealed class MetaItem
    {
        private static readonly Dictionary<string, MetaItem> items = [];

        #region Constructor

        // Constructor
        public MetaItem(string name, ItemAction action, DiceRoll baseDamage, Stat modifier, int bonus, Vector2 knockback, int maximum, int hp, int faith, int range, int durability, UpgradeHardness upgradeHardness, Sound? sound)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Bonus = bonus;
            this.Action = action;
            this.BaseDamage = baseDamage;
            this.Durability = durability;
            this.Knockback = knockback;
            this.Maximum = Math.Max(1, maximum);
            this.Faith = faith;
            this.HP = hp;
            this.Range = range;
            this.Modifier = modifier;
            this.LocalizedDescription = TextRepository.GetValue($"Item.{Name}.Description");
            this.LocalizedName = TextRepository.GetValue($"Item.{Name}.Name");
            this.UpgradeHardness = upgradeHardness;
            this.Sound = sound;
        }

        #endregion

        #region Static members

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        #endregion

        // Action
        public ItemAction Action { get; }

        // BaseDamage
        public DiceRoll BaseDamage { get; }

        // Bonus
        public int Bonus { get; }

        // Durability
        public int Durability { get; set; }

        // Faith
        public int Faith { get; }

        // HP
        public int HP { get; }

        // Knockback
        public Vector2 Knockback { get; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedName
        public string LocalizedName { get; }

        // Maximum
        public int Maximum { get; }

        // Modifier
        public Stat Modifier { get; }

        // Name
        public string Name { get; }

        // Range
        public int Range { get; }

        // Sound
        public Sound? Sound { get; }

        // ToString
        public override string ToString() => Name;

        // UpgradeHardness
        public UpgradeHardness UpgradeHardness { get; }
    }
}
