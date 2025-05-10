using Engendro;
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
        private readonly List<ItemEffect> upgradeEffects = [];

        #region Constructor

        // Constructor
        public MetaItem(string name, ItemCategory category, ItemAction action, DiceRoll baseDamage, Stat modifier, Vector2 knockback, int maximum, int hp, int faith, int range, int durability)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Category = category;
            this.Action = action;
            this.BaseDamage = baseDamage;
            this.Durability = durability;
            this.Knockback = knockback;
            this.Maximum = maximum;
            this.Faith = faith;
            this.HP = hp;
            this.Range = range;
            this.Modifier = modifier;
            this.LocalizedDescription = TextRepository.GetValue($"Item.{Name}.Description");
            this.LocalizedName = TextRepository.GetValue($"Item.{Name}.Name");
            this.UpgradeCosts = new ReadOnlyCollection<int>(upgradeCosts);
            this.UpgradeEffects = new ReadOnlyCollection<ItemEffect>(upgradeEffects);
        }

        #endregion

        #region Static members

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        #endregion

        // Action
        public ItemAction Action { get; set; }

        // AddUpgradeEffect
        public void AddUpgradeEffect(ItemEffect effect, int cost)
        {
            upgradeEffects.Add(effect);
            upgradeCosts.Add(cost);
        }

        // BaseDamage
        public DiceRoll BaseDamage { get; }

        // Category
        public ItemCategory Category { get; }

        // Durability
        public int Durability { get; set; }

        // Faith
        public int Faith { get; }

        // GetUpgradeDescription
        public string GetUpgradeDescription(int level)
        {
            if (level == 0 || level > upgradeEffects.Count)
                return string.Empty;

            var text = TextRepository.GetValue("ItemEffect." + UpgradeEffects[level - 1].GetType().Name);

            return text;
        }

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

        // MaximumLevel
        public int MaximumLevel => upgradeEffects.Count;

        // Modifier
        public Stat Modifier { get; }

        // Name
        public string Name { get; }

        // Range
        public int Range { get; }

        // ToString
        public override string ToString() => Name;

        // UpgradeCosts
        public ReadOnlyCollection<int> UpgradeCosts { get; }

        // UpgradeEffects
        public ReadOnlyCollection<ItemEffect> UpgradeEffects { get; }
    }
}
