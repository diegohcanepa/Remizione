using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

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
            this.PassiveEffectCooldown = metaItem.PassiveEffectCooldown;
        }

        #endregion

        #region Private members

        // InvalidateDisplayText
        private void InvalidateDisplayText()
        {
            if (!isDisplayTextDiry)
                return;

            var text = MetaItem.LocalizedDisplayName;

            // Add level
            if (Level > 0)
                text += $" +{Level}";

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
        public void ApplyDamage(GameThing target)
        {
            if (MetaItem.ApplyDamage(Owner, target))
            {
                if (MetaItem.Durability > 0 && Durability > 0)
                    Durability -= 1;
            }
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
                    count = Math.Clamp(value, MetaItem.AllowEmpty ? 0 : 1, 999);
                    isDisplayTextDiry = true;
                }
            }
        }

        // CriticalChance
        public int CriticalChance => MetaItem.CriticalChance + (Level * 5);

        // Discard
        public bool Discard()
        {
            if (MetaItem.PreventDiscard)
                return false;
            else
            {
                Remove();
                return true;
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

        // GetDisplayAmount
        public string GetDisplayAmount()
        {
            if (MetaItem.AllowEmpty)
                return count.ToString(CultureInfo.InvariantCulture);
            else
                return string.Empty;
        }

        // GetDisplayStat
        public string GetDisplayStat(ItemProperty property)
        {
            string value = string.Empty;

            // Health
            if (property == ItemProperty.Health && MetaItem.HP is DiceExpression exp)
                value = exp.GetValueRangeAsString();

            // Chance
            else if (property == ItemProperty.Chance)
                value = SkillChance.ToString(CultureInfo.InvariantCulture) + "%";

            return $"{Localization.GetValue(property)}: {value}";
        }

        // Index
        public int Index => Container.IndexOf(this);

        // IsSelected
        public bool IsSelected => Container.SelectedItem == this;

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
        public Actor Owner => Container.Owner;

        // PassiveEffectCooldown
        public int PassiveEffectCooldown { get; set; }

        /*
        // Update
        public void Update(GameTime gameTime)
        {
            if (MetaItem.PassiveEffectCooldown > 0)
            {
                PassiveEffectCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (PassiveEffectCooldown <= 0)
                {
                    PassiveEffectCooldown = MetaItem.PassiveEffectCooldown;
                    Use();
                }
            }
        }
        */

        // Range
        public int Range => MetaItem.Range;

        // Remove
        public void Remove()
        {
            Container.SelectPrevious();
            Container.Remove(this);
        }

        // Replenish
        public void Replenish()
        {
            if (MetaItem.ReplenishAmount > 0)
                Count = MetaItem.ReplenishAmount;
        }

        // Select
        public void Select() => Container.Select(this);

        // SkillChance
        public int SkillChance => MetaItem.SkillChance + (Level * 5);

        // ToString
        public override string ToString() => DisplayText;

        // Update
        public void Update(GameTime gameTime)
        {
            /*
            if (MetaItem.PassiveEffectCooldown > 0 && PassiveEffectCooldown <= 0)
            {
                PassiveEffectCooldown = MetaItem.PassiveEffectCooldown;
                Use();
            }
            else if (PassiveEffectCooldown > 0)
            {
                PassiveEffectCooldown -= 1; // Assuming this is called every frame, adjust as necessary
            }
            */
        }

        // Use
        public bool Use()
        {
            if (MetaItem.HP != null)
                Owner.HP += MetaItem.HP.Roll();

            if (!MetaItem.IsPassive && Count > 0)
            {
                if (Count == 1 && !MetaItem.AllowEmpty)
                    Container.Remove(this);
                else
                    Count--;
            }

            InvalidateDisplayText();

            return true;
        }
    }
}
