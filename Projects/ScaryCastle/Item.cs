using Adberration.Scripting;
using Engendro.Audio;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// Item
    /// </summary>
    public sealed class Item : IGameAction
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

        #region IGameAction interface

        string IGameAction.AnimationName => Definition.AnimationName;

        int IGameAction.AreaOfEffect => Definition.AreaOfEffect;

        ReadOnlyCollection<EffectDescriptor> IGameAction.EffectDescriptors => Definition.EffectDescriptors;

        InPlaceEffectType IGameAction.InPlaceEffectType => Definition.InPlaceEffectType;

        ProjectileDescriptor? IGameAction.Projectile => Definition.Projectile;

        Sound? IGameAction.SoundStart => Definition.SoundStart;

        Sound? IGameAction.SoundTrigger => Definition.SoundTrigger;

        ItemUsageScope IGameAction.UsageScope => Definition.UsageScope;


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

        // Consume
        public void Consume(Actor actor)
        {
            actor.Goo -= Definition.GooCost;

            if (Definition.IsDepletable)
            {
                Amount--;
                if (Amount <= 0)
                    Inventory.Remove(this);
            }

            InvalidateDisplayText();
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
    }
}
