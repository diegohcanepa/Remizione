using Engendro;
using Microsoft.Xna.Framework;
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
        public Item(Inventory inventory, MetaItem metaItem)
        {
            this.Inventory = inventory;
            this.MetaItem = metaItem;
            this.ConsumptionCooldown = metaItem.ConsumptionInterval;
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

        // ConsumptionCooldown
        public int ConsumptionCooldown { get; set; }

        // Count
        public int Count
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 0, MetaItem.IsStackable ? 99 : 1);
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

                return field;
            }

            private set;
        } = string.Empty;

        // Durability
        public Ratio Durability
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
            return Count.ToString(CultureInfo.InvariantCulture);
        }

        // GetDisplayStat
        public string GetDisplayStat(ItemProperty property)
        {
            string value = string.Empty;

            /*
            // Health
            if (property == ItemProperty.Health && MetaItem.Effect.HP is DiceExpression exp)
                value = exp.GetValueRangeAsString();

            // Chance
            else if (property == ItemProperty.Chance)
                value = MetaItem.SkillChance.ToString(CultureInfo.InvariantCulture) + "%";

                        */

            return $"{Localization.GetValue(property)}: {value}";
        }

        // Index
        public int Index => Inventory.IndexOf(this);

        // Inventory
        public Inventory Inventory { get; }

        // MetaItem
        public MetaItem MetaItem { get; }

        // Name
        public string Name => MetaItem.Name;

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

        // ToString
        public override string ToString()
        {
            return DisplayText;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (MetaItem.ConsumptionInterval > 0)
            {
                ConsumptionCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (ConsumptionCooldown <= 0)
                {
                    ConsumptionCooldown = MetaItem.ConsumptionInterval;
                    Use(null);
                }
            }
        }

        // Use
        public bool Use(GameThing? owner)
        {
            /*
            if (owner != null && MetaItem.Effect.HP != null)
                owner.HP += MetaItem.Effect.HP.Roll();

            switch (MetaItem.ConsumptionType)
            {
                // Quantity
                case ConsumptionType.Quantity:
                    Count--;
                    if (Count <= 0)
                        Inventory.Remove(this);
                    break;

                // Durability
                case ConsumptionType.Durability:
                    Durability -= MetaItem.DurabilityCost;
                    if (Durability <= 0)
                        Inventory.Remove(this);
                    break;

                // None
                case ConsumptionType.None:
                    break;

                default:
                    break;
            }

            InvalidateDisplayText();
            */

            return true;
        }
    }
}
