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
        public MetaItem(string name, MetaItemCategory category, int maximum)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Category = category;
            this.Maximum = Math.Max(1, maximum);
            this.LocalizedDescription = Localization.GetItemDescription(this);
            this.LocalizedName = Localization.GetItemName(this);
            this.Image = Atlases.UI.GetImage(Name);
        }

        #endregion

        // AllowEmpty
        public bool AllowEmpty { get; init; }

        // BaseDamage
        public DiceExpression? BaseDamage { get; init; }

        // Bonus
        public int Bonus { get; init; }

        // Category
        public MetaItemCategory Category { get; }

        // Durability
        public int Durability { get; init; }

        // Faith
        public DiceExpression? Faith { get; init; }

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        // HasUsageCost
        public bool HasUsageCost => HP != null || Faith != null || Tickets != null;

        // HP
        public DiceExpression? HP { get; init; }

        // Image
        public AtlasImage? Image { get; }

        // IsPassive
        public bool IsPassive => PassiveEffectCooldown > 0;

        // IsStackable
        public bool IsStackable => Maximum > 1;

        // IsThrowable
        public bool IsThrowable { get; init; }

        // Knockback
        public Vector2 Knockback { get; init; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedName
        public string LocalizedName { get; }

        // Maximum
        public int Maximum { get; init; }

        // Modifier
        public Stat Modifier { get; init; }

        // Name
        public string Name { get; }

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; init; }

        // Range
        public int Range { get; init; }

        // Sound
        public Sound? Sound { get; init; }

        // Tickets
        public DiceExpression? Tickets { get; init; }

        // ToString
        public override string ToString() => Name;
    }
}
