using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// Item
    /// </summary>
    public sealed class Item
    {
        #region Private fields

        private int count = 1;
        private string displayText = string.Empty;
        private int durability;
        private bool isDisplayTextDiry = true;
        private int level;

        #endregion

        #region Constructor

        // Constructor
        public Item(ItemContainer container, MetaItem metaItem)
        {
            this.Container = container;
            this.MetaItem = metaItem;
        }

        #endregion

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
            if (MetaItem.Maximum > 1)
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

        // ApplyDamage
        public void ApplyDamage(GameThing target, HitType hitType)
        {
            if (MetaItem.BaseDamage == null)
                return;

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

            target.TakeDamage(Container.Owner, damageAmount + MetaItem.Bonus, hitType, Knockback);

            target.ApplyDamage(Container.Owner);
        }

        // Container
        public ItemContainer Container { get; private set; }

        // Count
        public int Count
        {
            get => count;
            set
            {
                if (value != count)
                {
                    count = Math.Clamp(value, 1, MetaItem.Maximum);
                    isDisplayTextDiry = true;
                }
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

        // Faith
        public DiceExpression? Faith => MetaItem.Faith;

        // GetLocalizedInfo
        public string GetLocalizedInfo()
        {
            var values = new List<string>();

            if (MetaItem.BaseDamage != null)
                values.Add($"{Localization.GetLocalizedValue(ItemProperty.BaseDamage)}: {MetaItem.BaseDamage.MinimumValue + Level}-{MetaItem.BaseDamage.MaximumValue + Level}");

            if (HP != null)
                values.Add($"{TextRepository.GetValue($"DerivedStat.Spirit.Name")}: {HP.MinimumValue}-{HP.MaximumValue}");

            if (Faith != null)
                values.Add($"{TextRepository.GetValue($"DerivedStat.Faith.Name")}: {Faith.MinimumValue}-{Faith.MaximumValue}");

            return string.Join(" / ", values);
        }

        // GetUpgradeCost
        public int GetUpgradeCost()
        {
            var (baseCost, growthRate, powerFactor) = GetUpgradeParams(UpgradeHardness);
            double cost = baseCost * Math.Pow(Level, growthRate) * powerFactor;

            return (int)Math.Ceiling(cost);
        }

        // HP
        public DiceExpression? HP => MetaItem.HP;

        // IsStackFull
        public bool IsStackFull => MetaItem.Maximum == 1 || Count >= MetaItem.Maximum;

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
            if (MetaItem.Maximum > 1)
                Count = MetaItem.Maximum;
        }

        // ToString
        public override string ToString() => DisplayText;

        // Unread
        public bool Unread { get; set; }

        // UpgradeHardness
        public UpgradeHardness UpgradeHardness => MetaItem.UpgradeHardness;

        // Use
        public bool Use()
        {
            if (HP != null)
                Owner.HP += HP.Roll();

            if (Faith != null && Owner is Actor actor)
                actor.Faith += Faith.Roll();

            if (MetaItem.Maximum > 1)
            {
                if (Count == 1)
                    Container.Remove(this);
                else
                    Count--;
            }

            InvalidateDisplayText();

            return true;
        }
    }
}
