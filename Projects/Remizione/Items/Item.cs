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
            this.UseCooldown = metaItem.UseInterval;
        }

        #endregion

        #region Private members

        // InvalidateDisplayText
        private void InvalidateDisplayText()
        {
            if (!isDisplayTextDiry)
                return;

            var text = MetaItem.LocalizedName;
            if (MetaItem.Passive)
                text = GameSettings.PassiveSymbol + " " + text;

            // Add level
            if (Level > 0)
                text += $" +{Level}";

            // Count
            if (MetaItem.Maximum > 1)
            {
                if (MetaItem.Maximum == 999)
                    text += $" ({Count})";
                else
                    text += $" ({Count} / {MetaItem.Maximum})";
            }

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

        // GetLocalizedInfo
        public string GetLocalizedInfo()
        {
            var values = new List<string>();

            if (MetaItem.Passive)
                values.Add(GameSettings.PassiveSymbol + Localization.GetValue(ItemProperty.Passive));

            if (MetaItem.BaseDamage != null)
            {
                var value = $"{Localization.GetValue(ItemProperty.BaseDamage)}: {MetaItem.BaseDamage.GetValueRangeAsString(Level)}";
                values.Add(value);
            }

            if (MetaItem.HP != null)
            {
                var value = $"{TextRepository.GetValue($"DerivedStat.Spirit.Name")}: {MetaItem.HP.GetValueRangeAsString()}";
                values.Add(value);
            }

            if (MetaItem.Faith != null)
            {
                var value = $"{TextRepository.GetValue($"DerivedStat.Faith.Name")}: {MetaItem.Faith.GetValueRangeAsString()}";
                values.Add(value);
            }

            return string.Join(" / ", values);
        }

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

        // MeetUsageConditions
        public bool MeetUsageConditions()
        {
            var actor = Owner as Actor;

            // Create
            if (MetaItem.IsStackable && MetaItem.Action == ItemAction.Create)
                return IsStackFull && actor != null;

            // Not enough HP
            if (MetaItem.HP is DiceExpression hpExp && hpExp.FixedValue < 0 && Owner.HP <= Math.Abs(hpExp.FixedValue))
                return false;

            // Not enough faith
            if (actor != null)
            {
                if (MetaItem.Faith is DiceExpression faithExp && faithExp.FixedValue < 0 && actor.Faith <= faithExp.FixedValue)
                    return false;
            }

            return true;
        }

        // MetaItem
        public MetaItem MetaItem { get; }

        // Name
        public string Name => MetaItem.Name;

        // Owner
        public GameThing Owner => Container.Owner;

        // Update
        public void Update(GameTime gameTime)
        {
            if (MetaItem.UseInterval > 0)
            {
                UseCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (UseCooldown <= 0)
                {
                    UseCooldown = MetaItem.UseInterval;
                    Use();
                }
            }
        }

        // Range
        public int Range => MetaItem.Range;

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

        // Use
        public bool Use()
        {
            if (!MeetUsageConditions())
                return false;

            if (MetaItem.HP != null)
                Owner.HP += MetaItem.HP.Roll();

            if (MetaItem.Faith != null && Owner is Actor actor)
                actor.Faith += MetaItem.Faith.Roll();

            var action = MetaItem.Action;

            if (MetaItem.Maximum > 1 && action != ItemAction.Create)
            {
                if (Count == 1)
                    Container.Remove(this);
                else
                    Count--;
            }
            else if (action == ItemAction.Create || action == ItemAction.Use)
                Container.Remove(this);

            InvalidateDisplayText();

            return true;
        }

        // UseCooldown
        public int UseCooldown { get; set; }
    }
}
