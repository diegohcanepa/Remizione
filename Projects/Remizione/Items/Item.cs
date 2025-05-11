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
        private string displayText = string.Empty;
        private int durability;
        private bool isDisplayTextDiry = true;
        private int level;

        // Constructor
        public Item(ItemContainer container, MetaItem metaItem)
        {
            this.Container = container;
            this.MetaItem = metaItem;
            this.IconImage = Atlases.UI.GetImage(MetaItem.ToString()) ?? Atlases.UI.MissingItem;
        }

        #region Private members

        // GetUgradeParams
        private static (double baseCost, double growthRate, double powerFactor) GetUpgradeParams(UpgradeHardness hardness)
        {
            return hardness switch
            {
                UpgradeHardness.Easy => (8, 1.3, 1.0),
                UpgradeHardness.Normal => (10, 1.5, 1.2),
                UpgradeHardness.Hard => (12, 1.8, 1.4),
                _ => throw new ArgumentOutOfRangeException(nameof(hardness), "Unknown hardness value")
            };
        }

        // InvalidateDisplayText
        private void InvalidateDisplayText()
        {
            if (!isDisplayTextDiry)
                return;

            var text = MetaItem.LocalizedName;

            // Add level
            if (Level > 0)
                text += $" +{Level}";

            // Count
            if (MetaItem.Maximum > 0 && Count > 0)
                text += $" ({Count} / {MetaItem.Maximum})";

            // Durability state
            else if (MetaItem.Durability > 0)
            {
                var ratio = Durability / MetaItem.Durability;

                if (ratio >= .85f)
                    text += $" ({TextRepository.GetValue("@DurabilityState.Sturdy")})";

                else if (ratio >= .75f)
                    text += $" ({TextRepository.GetValue("@DurabilityState.Used")})";

                else if (ratio >= .5f)
                    text += $" ({TextRepository.GetValue("@DurabilityState.Worn")})";

                else if (ratio >= .25f)
                    text += $" ({TextRepository.GetValue("@DurabilityState.Cracked")})";

                else
                    text += $" ({TextRepository.GetValue("@DurabilityState.Broken")})";
            }

            displayText = text;

            isDisplayTextDiry = false;
        }

        #endregion

        // BeginUse
        public void BeginUse()
        {
            Owner.Spirit += Spirit;

            if (Owner is Actor actor)
                actor.Faith += Faith;
        }

        // Consume
        public bool Consume()
        {
            if (Count > 0)
            {
                BeginUse();
                Count--;
                InvalidateDisplayText();
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
                
                isDisplayTextDiry = true;
            }
        }

        // DisplayText
        public string DisplayText
        {
            get
            {
                if (isDisplayTextDiry)
                    InvalidateDisplayText();

                return displayText;
            }
        }

        // Durability
        public int Durability
        {
            get => durability;
            set
            {
                this.durability = value;
                if (durability < 0)
                    durability = 0;
                
                isDisplayTextDiry = true;
            }
        }

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

            target.ApplyDamage(Container.Owner);
        }

        // Faith
        public int Faith => MetaItem.Faith;

        // GetUpgradeCost
        public int GetUpgradeCost()
        {
            var (baseCost, growthRate, powerFactor) = GetUpgradeParams(UpgradeHardness);
            double cost = baseCost * Math.Pow(Level, growthRate) * powerFactor;

            return (int)Math.Ceiling(cost);
        }

        // IconImage
        public AtlasImage IconImage { get; }

        // IsActive
        public bool IsActive => Container.SelectedItem == this;

        // Knockback
        public Vector2 Knockback => MetaItem.Knockback;

        // Level
        public int Level
        {
            get => level;
            set
            {
                if (value != level)
                {
                    this.level = value;
                    isDisplayTextDiry = true;
                }
            }
        }

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

        // Spirit
        public int Spirit => MetaItem.Spirit;

        // ToString
        public override string ToString() => MetaItem.ToString();

        // Unread
        public bool Unread { get; set; }

        // UpgradeHardness
        public UpgradeHardness UpgradeHardness => MetaItem.UpgradeHardness;
    }
}
