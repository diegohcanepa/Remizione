
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

        #region Constructor

        // Constructor
        public MetaItem(string name, InventoryCategory category, int maximum)
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

        // Action
        public ItemAction Action { get; init; }

        // AllowEmpty
        public bool AllowEmpty { get; init; }

        // ApplyDamage
        public bool ApplyDamage(GameThing attacker, GameThing target)
        {
            if (Damage == null)
                return false;

            int damageAmount = Damage.Roll();

            target.TakeDamage(attacker, damageAmount, Knockback, ImpactWord);
            target.ApplyDamage(attacker);

            return true;
        }

        // Damage
        public DiceExpression? Damage { get; init; }

        // Bonus
        public int Bonus { get; init; }

        // Category
        public InventoryCategory Category { get; }

        // Chance
        public int Chance { get; init; }

        // Durability
        public int Durability { get; init; }

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        // Health
        public DiceExpression? Health { get; init; }

        // Image
        public AtlasImage? Image { get; }

        // ImpactWord
        public ImpactWordKind ImpactWord { get; init; }

        // IsPassive
        public bool IsPassive => PassiveEffectCooldown > 0;

        // IsStackable
        public bool IsStackable => Maximum > 1;

        // Knockback
        public Vector2 Knockback { get; init; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedName
        public string LocalizedName { get; }

        // Maximum
        public int Maximum { get; init; }

        // Modifier
        public StatModifier Modifier { get; init; }

        // Name
        public string Name { get; }

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; init; }

        // PreventDiscard
        public bool PreventDiscard { get; init; }

        // Range
        public int Range { get; init; }

        // ReplenishPerRoom
        public bool ReplenishPerRoom { get; init; }

        // Sound
        public Sound? Sound { get; init; }

        // ToString
        public override string ToString() => Name;
    }
}
