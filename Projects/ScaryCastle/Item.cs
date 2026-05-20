using Adberration.Scripting;
using Engendro;
using Microsoft.Xna.Framework;
using System.Globalization;
using System.Linq;

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
        public Item(PlayerInventory inventory, ItemDefinition definition)
        {
            this.Inventory = inventory;
            this.Definition = definition;
            this.ConsumptionCooldown = definition.ConsumptionInterval;
            this.Script = inventory.Session.ScriptLibrary.FindRoutine($"{Definition.Name}Outcome");
        }

        #endregion

        #region Private members

        // InvalidateDisplayText
        private void InvalidateDisplayText()
        {
            if (!isDisplayTextDiry)
                return;

            var text = Definition.DisplayName;

            // Durability state
            if (Definition.Durability > 0)
            {
                var ratio = Durability / Definition.Durability;

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

        // ApplyEffects
        public bool ApplyEffects(GameThing source, GameThing? target, EffectContext context)
        {
            // Play sound
            if (Definition.Sound != null)
                source.PlaySound(Definition.Sound);

            if (Definition.AreaRange == 0)
            {
                if (target != null)
                {
                    EffectDescriptor.Apply(Definition.EffectDescriptors, source, target, context);

                    // Any food item remove poison status
                    if (Definition.Category == ItemCategory.Food && target.Condition == ConditionType.Poison)
                    {
                        var hasPoisonStatus = false;
                        for (var i = 0; i < Definition.EffectDescriptors.Count; i++)
                        {
                            if (Definition.EffectDescriptors[i].Condition == ConditionType.Poison)
                            {
                                hasPoisonStatus = true;
                                break;
                            }
                        }

                        if (!hasPoisonStatus)
                            target.ClearCondition();
                    }
                }
            }
            else if (source.Room is GameRoom room)
            {
                foreach (var potentialTarget in room.Children.OfType<GameThing>())
                {
                    if (source.DistanceTo(potentialTarget) < Definition.AreaRange)
                        EffectDescriptor.Apply(Definition.EffectDescriptors, source, potentialTarget, context);
                }
            }

            ComputeUse();

            return true;
        }

        // ComputeUse
        public bool ComputeUse()
        {
            // Quantity
            if (Definition.ConsumptionType == ConsumptionType.Quantity)
            {
                Count--;
                if (Count <= 0)
                    Inventory.Remove(this);
            }
            else if (Definition.ConsumptionType == ConsumptionType.Durability)
            {
                Durability -= Definition.DurabilityCost;
                if (Durability <= 0)
                    Inventory.Remove(this);
            }

            InvalidateDisplayText();

            return true;
        }

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
                    field = int.Clamp(value, 0, Definition.IsStackable ? 99 : 1);
                    isDisplayTextDiry = true;
                    if (field == 0)
                        Inventory.Remove(this);
                    Inventory.InvalidateContentVersion();
                }
            }
        }

        // Definition
        public ItemDefinition Definition { get; }

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

        // Index
        public int Index => Inventory.IndexOf(this);

        // Inventory
        public PlayerInventory Inventory { get; }

        // Name
        public string Name => Definition.Name;

        // Remove
        public void Remove()
        {
            Inventory.Remove(this);
        }

        // Script
        public Script? Script { get; }

        // ToString
        public override string ToString()
        {
            return DisplayText;
        }

        // Update
        public void Update(GameTime gameTime)
        {
            if (Definition.ConsumptionInterval > 0)
            {
                ConsumptionCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                if (ConsumptionCooldown <= 0)
                {
                    ConsumptionCooldown = Definition.ConsumptionInterval;
                    //Use( );
                }
            }
        }
    }
}
