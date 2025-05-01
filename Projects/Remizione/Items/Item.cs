using Engendro;
using Microsoft.Xna.Framework;
using System;

namespace Remizione
{
    /// <summary>
    /// Item
    /// </summary>
    public sealed class Item
    {
        private int count;
        private int maximum;
        private readonly string nameString;

        // Constructor
        public Item(ItemStorage storage, MetaItem metaItem)
        {
            this.nameString = metaItem.Name.ToString();

            this.Storage = storage;
            this.MetaItem = metaItem;
            this.Anger = metaItem.Anger;
            this.Category = metaItem.Category;
            this.IconImage = Atlases.UI.GetImage(nameString) ?? Atlases.UI.MissingItem;
            this.BaseDamage = metaItem.BaseDamage;
            this.Knockback = metaItem.Knockback;
            this.Maximum = metaItem.Maximum;
            this.HP = metaItem.HP;
            this.Range = metaItem.Range;
        }

        // Anger
        public int Anger { get; }

        // BaseDamage
        public DiceRoll BaseDamage { get; }

        // BeginUse
        public void BeginUse()
        {
            Owner.HP += HP;

            if (Owner is Actor actor)
            {
                if (actor.Session.CombatManager.IsActive)
                    actor.Anger += Anger;
            }

            if (Level > 0 && MetaItem.UpgradeEffects.Count > 0)
            {
                for (var i = 1; i <= Level; i++)
                {
                    if (MetaItem.UpgradeEffects[i].EffectTiming == ItemEffectTiming.OnBeginUse)
                        MetaItem.UpgradeEffects[i].Apply(Owner);
                }
            }
        }

        // Category
        public ItemCategory Category { get; }

        // Consume
        public bool Consume()
        {
            if (Count > 0)
            {
                Count--;
                return true;
            }

            return false;
        }

        // Count
        public int Count
        {
            get => count;
            set
            {
                this.count = value;
                if (Maximum > 0 && count > Maximum)
                    count = Maximum;
            }
        }

        // EndUse
        public void EndUse(GameThing target)
        {
            if (BaseDamage != DiceRoll.Empty)
            {
                int damageAmount;
                if (MetaItem.Anger < 0 && Owner is Actor actor && actor.Anger < 0)
                    damageAmount = BaseDamage.MinimumValue;
                else
                    damageAmount = BaseDamage.Roll();

                target.TakeDamage(Storage.Owner, damageAmount, damageAmount == BaseDamage.MaximumValue, Knockback);
            }

            if (Level > 0 && MetaItem.UpgradeEffects.Count > 0)
            {
                for (var i = 1; i <= Level; i++)
                {
                    if (MetaItem.UpgradeEffects[i].EffectTiming == ItemEffectTiming.OnEndUse)
                        MetaItem.UpgradeEffects[i].Apply(target);
                }

                for (var i = 1; i <= Level; i++)
                {
                    if (MetaItem.UpgradeEffects[i].EffectTiming == ItemEffectTiming.AfterAllEffects)
                        MetaItem.UpgradeEffects[i].Apply(target);
                }
            }

            target.ApplyDamage(Storage.Owner);
        }

        // GetLocalizedUpgradeDescription
        public string? GetLocalizedUpgradeDescription()
        {
            if (MetaItem.UpgradeEffects.Count == 0 || Level == MetaItem.UpgradeEffects.Count)
                return null;

            return MetaItem.UpgradeEffects[Level].GetLocalizedDescription();
        }

        // HasUpgrade
        public bool HasUpgrade => Level < MetaItem.UpgradeEffects.Count;

        // HP
        public int HP { get; }

        // IconImage
        public AtlasImage IconImage { get; }

        // IsActive
        public bool IsActive => Storage.SelectedItem == this;

        // Knockback
        public Vector2 Knockback { get; }

        // Level
        public int Level { get; set; }

        // Maximum
        public int Maximum
        {
            get => maximum;
            set
            {
                this.maximum = value;
                if (value > 0 && count > maximum)
                    count = maximum;
            }
        }

        // MetaItem
        public MetaItem MetaItem { get; }

        // Name
        public ItemName Name => MetaItem.Name;

        // Owner
        public GameThing Owner => Storage.Owner;

        // Range
        public int Range { get; }

        // Replenish
        public void Replenish()
        {
            if (Maximum > 0)
                Count = Maximum;
        }

        // Storage
        public ItemStorage Storage { get; }

        // ToString
        public override string ToString() => nameString;

        // Unread
        public bool Unread { get; set; }

        // UpgradeCost
        public int UpgradeCost
        {
            get
            {
                if (MetaItem.UpgradeEffects.Count == 0 || Level == MetaItem.UpgradeEffects.Count)
                    return -1;

                return MetaItem.UpgradeCosts[Level];
            }
        }
    }
}
