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
        public MetaItem(string name, MetaItemCategory category, int passiveEffectCooldown, DiceExpression? baseDamage, Stat modifier, int bonus, Vector2 knockback, int maximum, DiceExpression? hp, DiceExpression? faith, int range, int durability, Sound? sound)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Category = category;
            this.Bonus = bonus;
            this.PassiveEffectCooldown = passiveEffectCooldown;
            this.BaseDamage = baseDamage;
            this.Durability = durability;
            this.Knockback = knockback;
            this.Maximum = Math.Max(1, maximum);
            this.Faith = faith;
            this.HP = hp;
            this.Range = range;
            this.Modifier = modifier;
            this.LocalizedDescription = Localization.GetItemDescription(this);
            this.LocalizedName = Localization.GetItemName(this);
            this.Sound = sound;
            this.Image = Atlases.UI.GetImage(Name);
        }

        #endregion

        // AllowEmpty
        public bool AllowEmpty { get; init; }

        // BaseDamage
        public DiceExpression? BaseDamage { get; }

        // Bonus
        public int Bonus { get; }

        // Category
        public MetaItemCategory Category { get; }

        // Durability
        public int Durability { get; set; }

        // Faith
        public DiceExpression? Faith { get; }

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        // HP
        public DiceExpression? HP { get; }

        // Image
        public AtlasImage? Image { get; }

        // IsPassive
        public bool IsPassive => PassiveEffectCooldown > 0;

        // IsStackable
        public bool IsStackable => Maximum > 1;

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

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; }

        // Range
        public int Range { get; }

        // SacrificeReward
        public SacrificeReward SacrificeReward { get; init; } = SacrificeReward.Faith;

        // Sound
        public Sound? Sound { get; }

        // ToString
        public override string ToString() => Name;
    }
}
