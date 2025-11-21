
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
        public MetaItem(string name, ItemCategory category, LootTag[] tags, LootTag[] requiredTags, LootTag[] excludeTags)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (Enum.IsDefined(typeof(ItemRealm), name))
                throw new InvalidOperationException($"The name '{name}' cannot be used because it is an item realm.");

            if (Enum.IsDefined(typeof(ItemCategory), name))
                throw new InvalidOperationException($"The name '{name}' cannot be used because it is an item category.");

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            this.Name = name;
            this.Category = category;
            this.LocalizedDescription = Localization.GetItemDescription(this);
            this.LocalizedDisplayName = Localization.GetItemName(this);
            this.Image = Atlases.UI.GetImage(Name);
            this.Tags = new(tags);
            this.RequiredTags = new(requiredTags);
            this.ExcludeTags = new(excludeTags);
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

        // BaseWeight
        public float BaseWeight { get; init; }

        // Category
        public ItemCategory Category { get; }

        // CoinItemName
        public const string CoinItemName = "Coin";

        // CriticalChance
        public int CriticalChance { get; init; }

        // Damage
        public DiceExpression? Damage { get; init; }

        // DamageType
        public DamageType DamageType { get; init; }

        // Durability
        public int Durability { get; init; }

        // ExcludeTags
        public ReadOnlyCollection<LootTag> ExcludeTags { get; }

        // Find
        public static MetaItem? Find(string name)
        {
            if (items.TryGetValue(name, out var result))
                return result;
            else
                return null;
        }

        // FindNotNull
        public static MetaItem FindNotNull(string name)
        {
            return Find(name) ?? throw new InvalidOperationException($"MetaItem '{name}' not found.");
        }

        // GetItems
        public static List<MetaItem> GetItems(ItemCategory category)
        {
            var result = new List<MetaItem>();

            foreach (var item in items.Values)
            {
                if (item.Category == category)
                    result.Add(item);
            }

            return result;
        }

        // GetItems
        public static List<MetaItem> GetItems(ItemRealm realm)
        {
            var result = new List<MetaItem>();

            foreach (var item in items.Values)
            {
                if (item.Realm == realm)
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
        public bool IsEquipment => Category == ItemCategory.LeftHand || Category == ItemCategory.RightHand || Category == ItemCategory.Gadgets;

        // IsPassive
        public bool IsPassive => PassiveEffectCooldown > 0;

        // IsSouvenir
        public bool IsSouvenir { get; init; }

        // IsStackable
        public bool IsStackable { get; init; }

        // Knockback
        public Vector2 Knockback { get; init; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedDisplayName
        public string LocalizedDisplayName { get; }

        // Name
        public string Name { get; }

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; init; }

        // PickupSound
        public Sound? PickupSound { get; init; }

        // PreventDiscard
        public bool PreventDiscard { get; init; }

        // Quality
        public int Quality { get; init; }

        // Range
        public int Range { get; init; }

        // Realm
        public ItemRealm Realm { get; init; }

        // RequiredTags
        public ReadOnlyCollection<LootTag> RequiredTags { get; }

        // SkillChance
        public int SkillChance { get; init; }

        // Sound
        public Sound? Sound { get; init; }

        // StackMode
        public StackMode StackMode { get; init; }

        // Tags
        public ReadOnlyCollection<LootTag> Tags { get; }

        // ToString
        public override string ToString() => Name;

        // Unlocked
        public bool Unlocked { get; init; }
    }
}
