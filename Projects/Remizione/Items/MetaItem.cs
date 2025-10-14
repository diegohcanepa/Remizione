
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
        public MetaItem(string name, InventoryCategory category)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Category = category;
            this.LocalizedDescription = Localization.GetItemDescription(this);
            this.LocalizedDisplayName = Localization.GetItemName(this);
            this.Image = Atlases.UI.GetImage(Name);
        }

        #endregion

        #region Private members

        // CalculateKnockback
        private Vector2 CalculateKnockback(ActorSize size)
        {
            var knockbackBase = Knockback;

            return size switch
            {
                // Small
                ActorSize.Small => knockbackBase * 1.5f,
                // Medium
                ActorSize.Medium => knockbackBase * 1f,
                // Large
                ActorSize.Large => knockbackBase * .3f,
                _ => knockbackBase,
            };
        }

        #endregion

        // Action
        public ItemAction Action { get; init; }

        // AllItems
        public static IEnumerable<MetaItem> AllItems => items.Values;

        // AllowEmpty
        public bool AllowEmpty { get; init; }

        // ApplyDamage
        public bool ApplyDamage(GameThing attacker, GameThing target)
        {
            if (Damage == null)
                return false;

            int damageAmount = Damage.Roll();

            var finalKnockback = Knockback;
            if (target is Actor actor)
                finalKnockback = CalculateKnockback(actor.BodySize);

            target.TakeDamage(attacker, damageAmount, DamageType, DiceExpression.Dice100.Roll() <= CriticalChance, finalKnockback, ImpactWord);

            return true;
        }

        // Category
        public InventoryCategory Category { get; }

        // CriticalChance
        public int CriticalChance { get; init; }

        // Damage
        public DiceExpression? Damage { get; init; }

        // DamageType
        public DamageType DamageType { get; init; }

        // Durability
        public int Durability { get; init; }

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        // FindNotNull
        public static MetaItem FindNotNull(string name) => Find(name) ?? throw new InvalidOperationException($"MetaItem '{name}' not found.");

        // GetItems
        public static List<MetaItem> GetItems(InventoryCategory category)
        {
            var result = new List<MetaItem>();

            foreach (var item in items.Values)
            {
                if (item.Category == category)
                    result.Add(item);
            }

            return result;
        }

        // HP
        public DiceExpression? HP { get; init; }

        // Image
        public AtlasImage? Image { get; }

        // ImpactWord
        public ImpactWordName ImpactWord { get; init; }

        // IsEquipment
        public bool IsEquipment => Category == InventoryCategory.Junk || Category == InventoryCategory.Thingies || Category == InventoryCategory.Trinkets;

        // IsPassive
        public bool IsPassive => PassiveEffectCooldown > 0;

        // Knockback
        public Vector2 Knockback { get; init; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; }

        // MagneticCardName
        public const string MagneticCardName = "MagneticCard";

        // Name
        public string Name { get; }

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; init; }

        // PreventDiscard
        public bool PreventDiscard { get; init; }

        // Range
        public int Range { get; init; }

        // ReplenishAmount
        public int ReplenishAmount { get; init; }

        // SkillChance
        public int SkillChance { get; init; }

        // Sound
        public Sound? Sound { get; init; }

        // ToString
        public override string ToString() => Name;
    }
}
