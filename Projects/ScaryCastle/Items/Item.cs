using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// Item
    /// </summary>
    public sealed class Item
    {
        #region Private fields

        private bool isDisplayTextDiry = true;

        #endregion

        #region Constructor

        // Constructor
        public Item(Inventory pilgrimSack, MetaItem metaItem)
        {
            this.Inventory = pilgrimSack;
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

            // Durability state
            if (MetaItem.Durability > 0)
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

            DisplayText = text;

            isDisplayTextDiry = false;
        }

        #endregion

        // ApplyDamage
        public void ApplyDamage(GameThing attacker, GameThing target)
        {
            if (MetaItem.Effect.ApplyDamage(attacker, target))
            {
                if (MetaItem.Durability > 0 && Durability > 0)
                    Durability -= 1;
            }
        }

        // Count
        public int Count
        {
            get;
            set
            {
                if (value != field)
                {
                    field = Math.Clamp(value, MetaItem.StackMode == StackMode.Persistent ? 0 : 1, 999);
                    isDisplayTextDiry = true;
                }
            }
        } = 1;

        // Discard
        public bool Discard()
        {
            if (MetaItem.PreventDiscard)
            {
                return false;
            }
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

                return field;
            }

            private set;
        } = string.Empty;

        // Durability
        public int Durability
        {
            get;
            set
            {
                field = value;
                if (field < 0)
                    field = 0;

                isDisplayTextDiry = true;
            }
        }

        // GetDisplayAmount
        public string GetDisplayAmount()
        {
            if (MetaItem.StackMode != StackMode.None)
                return Count.ToString(CultureInfo.InvariantCulture);
            else
                return string.Empty;
        }

        // GetDisplayStat
        public string GetDisplayStat(ItemProperty property)
        {
            string value = string.Empty;

            // Health
            if (property == ItemProperty.Health && MetaItem.Effect.HP is DiceExpression exp)
                value = exp.GetValueRangeAsString();

            // Chance
            else if (property == ItemProperty.Chance)
                value = SkillChance.ToString(CultureInfo.InvariantCulture) + "%";

            return $"{Localization.GetValue(property)}: {value}";
        }

        // Index
        public int Index => Inventory.IndexOf(this);

        // Inventory
        public Inventory Inventory { get; private set; }

        // IsEquipped
        public bool IsEquipped => Inventory.FindEquippedItem(MetaItem.Category) == this;

        // IsSelected
        public bool IsSelected => Inventory.SelectedItem == this;

        // Knockback
        public Vector2 Knockback => MetaItem.Effect.Knockback;

        // MetaItem
        public MetaItem MetaItem { get; }

        // Name
        public string Name => MetaItem.Name;

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
            Inventory.Remove(this);
        }

        // Select
        public void Select()
        {
            Inventory.Select(this);
        }

        // SkillChance
        public int SkillChance => MetaItem.SkillChance;

        // ToString
        public override string ToString()
        {
            return DisplayText;
        }

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
        public bool Use(GameThing? owner)
        {
            if (owner != null && MetaItem.Effect.HP != null)
                owner.HP += MetaItem.Effect.HP.Roll();

            if (!MetaItem.IsPassive && Count > 0)
            {
                if (Count == 1 && MetaItem.StackMode != StackMode.Persistent)
                    Inventory.Remove(this);
                else
                    Count--;
            }

            InvalidateDisplayText();

            return true;
        }
    }
}
