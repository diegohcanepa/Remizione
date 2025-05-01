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
        private static readonly Dictionary<ItemName, MetaItem> types = [];
        private readonly List<int> upgradeCosts = [];
        private readonly List<ItemEffect> upgradeEffects = [];

        #region Constructor

        // Constructor
        public MetaItem(ItemName name, ItemCategory category, ItemAction action, DiceRoll baseDamage, Vector2 knockback, int maximum, int hp, int faith, int range, int anger)
        {
            if (name == ItemName.None)
                throw new InvalidOperationException("Item must have a name.");

            this.Name = name;
            this.Category = category;
            this.Action = action;
            this.BaseDamage = baseDamage;
            this.Knockback = knockback;
            this.Maximum = maximum;
            this.Faith = faith;
            this.HP = hp;
            this.Range = range;
            this.Anger = anger;

            this.UpgradeCosts = new ReadOnlyCollection<int>(upgradeCosts);
            this.UpgradeEffects = new ReadOnlyCollection<ItemEffect>(upgradeEffects);
        }

        #endregion

        #region Static members

        // Find
        public static MetaItem? Find(ItemName name) => types.TryGetValue(name, out var result) ? result : null;

        // Register
        public static void Register(MetaItem metaItem)
        {
            if (!types.ContainsKey(metaItem.Name))
                types[metaItem.Name] = metaItem;
        }

        #endregion

        // Action
        public ItemAction Action { get; set; }

        // AddUpgradeEffect
        public void AddUpgradeEffect(ItemEffect effect, int cost)
        {
            upgradeEffects.Add(effect);
            upgradeCosts.Add(cost);
        }

        // Anger
        public int Anger { get; }

        // BaseDamage
        public DiceRoll BaseDamage { get; }

        // Category
        public ItemCategory Category { get; }

        // GetUpgradeDescription
        public string GetUpgradeDescription(int level)
        {
            if (level == 0 || level > upgradeEffects.Count)
                return string.Empty;

            var text = TextRepository.GetValue("ItemEffects." + UpgradeEffects[level - 1].GetType().Name);

            return text;
        }

        // Faith
        public int Faith { get; }

        // HP
        public int HP { get; }

        // Knockback
        public Vector2 Knockback { get; }

        // Maximum
        public int Maximum { get; }

        // MaximumLevel
        public int MaximumLevel => upgradeEffects.Count;

        // Name
        public ItemName Name { get; }

        // Range
        public int Range { get; }

        // ToString
        public override string ToString() => Name.ToString();

        // UpgradeCosts
        public ReadOnlyCollection<int> UpgradeCosts { get; }

        // UpgradeEffects
        public ReadOnlyCollection<ItemEffect> UpgradeEffects { get; }
    }
}
