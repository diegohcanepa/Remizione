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
            this.Script = inventory.Session.ScriptLibrary.FindRoutine($"{Definition.Name}Outcome");
            this.Amount = definition.InitialAmount;
        }

        #endregion

        #region Private members

        // InvalidateDisplayText
        private void InvalidateDisplayText()
        {
            if (!isDisplayTextDiry)
                return;

            DisplayText = Definition.DisplayName;

            isDisplayTextDiry = false;
        }

        #endregion

        // Amount
        public int Amount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 0, Definition.IsStackable || Definition.IsDepletable ? 99 : 1);
                    isDisplayTextDiry = true;
                    if (field == 0)
                        Inventory.Remove(this);
                    Inventory.InvalidateContentVersion();
                }
            }
        }

        // ComputeUse
        public bool ComputeUse()
        {
            if (Definition.IsDepletable)
            {
                Amount--;
                if (Amount <= 0)
                    Inventory.Remove(this);
            }
            
            InvalidateDisplayText();

            return true;
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

        // GetDisplayAmount
        public string GetDisplayAmount()
        {
            return Amount.ToString(CultureInfo.InvariantCulture);
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

        // Use
        public bool Use(GameThing source, GameThing? target, EffectContext context)
        {
            // Play sound
            if (Definition.Sound != null)
                source.PlaySound(Definition.Sound);

            if (Definition.AreaRange == 0)
            {
                EffectDescriptor.Apply(Definition.EffectDescriptors, source, target, context);

                if (target != null)
                {
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
    }
}
