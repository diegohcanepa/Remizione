using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// MetaItem
    /// </summary>
    public sealed class MetaItem
    {
        private static readonly Dictionary<string, MetaItem> items = [];
        private readonly List<int> upgradeCosts = [];

        #region Constructor

        // Constructor
        public MetaItem(string name, ItemCategory category, DiceRoll baseDamage, Stat modifier, Vector2 knockback, int maximum, int spirit, int faith, int range, int durability, UpgradeHardness upgradeHardness)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Category = category;
            this.BaseDamage = baseDamage;
            this.Durability = durability;
            this.Knockback = knockback;
            this.Maximum = maximum;
            this.Faith = faith;
            this.Spirit = spirit;
            this.Range = range;
            this.Modifier = modifier;
            this.LocalizedDescription = TextRepository.GetValue($"Item.{Name}.Description");
            this.LocalizedName = TextRepository.GetValue($"Item.{Name}.Name");
            this.UpgradeHardness = upgradeHardness;
        }

        #endregion

        #region Static members

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        #endregion

        // BaseDamage
        public DiceRoll BaseDamage { get; }

        // BaseUpgradeCost
        public int BaseUpgradeCost { get; }

        // Category
        public ItemCategory Category { get; }

        // Durability
        public int Durability { get; set; }

        // Faith
        public int Faith { get; }

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

        // Spirit
        public int Spirit { get; }

        // ToString
        public override string ToString() => Name;

        // UpgradeHardness
        public UpgradeHardness UpgradeHardness { get; }
    }
}
