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

        // Constructor
        public Item(ItemContainer container, MetaItem metaItem)
        {
            this.Container = container;
            this.MetaItem = metaItem;
            this.IconImage = Atlases.UI.GetImage(MetaItem.ToString()) ?? Atlases.UI.MissingItem;
        }

        // BeginUse
        public void BeginUse()
        {
            Owner.HP += HP;

            if (Owner is Actor actor)
            {
                if (actor.Session.CombatManager.IsActive)
                    actor.Faith += Faith;
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

        // Container
        public ItemContainer Container { get; }

        // Count
        public int Count
        {
            get => count;
            set
            {
                this.count = value;
                if (MetaItem.Maximum > 0 && count > MetaItem.Maximum)
                    count = MetaItem.Maximum;
            }
        }

        // Durability
        public int Durability { get; set; }

        // EndUse
        public void EndUse(GameThing target, HitType hitType)
        {
            if (MetaItem.BaseDamage != DiceRoll.Empty)
            {
                var actor = Owner as Actor;
                int damageAmount;

                // Faith penalty
                if (actor != null && (actor.Faith <= 0 || hitType == HitType.Glancing))
                {
                    damageAmount = MetaItem.BaseDamage.MinimumValue;
                }
                else
                {
                    damageAmount = MetaItem.BaseDamage.Roll();

                    if (actor != null)
                        damageAmount += actor.Stats.GetModifier(MetaItem.Modifier);

                    if (hitType == HitType.Critical)
                        damageAmount += Math.Max(MetaItem.BaseDamage.Roll(), MetaItem.BaseDamage.MaximumValue / 2);
                }

                if (MetaItem.Durability > 0 && Durability > 0)
                    Durability -= 1;

                target.TakeDamage(Container.Owner, damageAmount, hitType, Knockback);
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

            target.ApplyDamage(Container.Owner);
        }

        // Faith
        public int Faith => MetaItem.Faith;

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
        public bool IsActive => Container.SelectedItem == this;

        // Knockback
        public Vector2 Knockback => MetaItem.Knockback;

        // Level
        public int Level { get; set; }

        // MetaItem
        public MetaItem MetaItem { get; }

        // Name
        public string Name => MetaItem.Name;

        // Owner
        public GameThing Owner => Container.Owner;

        // Range
        public int Range { get; }

        // Replenish
        public void Replenish()
        {
            if (MetaItem.Maximum > 0)
                Count = MetaItem.Maximum;
        }

        // ToString
        public override string ToString() => MetaItem.ToString();

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
