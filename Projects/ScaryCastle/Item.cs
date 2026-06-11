using Adberration.Scripting;
using Engendro.Audio;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ScaryCastle
{
    /// <summary>
    /// Item
    /// </summary>
    public sealed class Item : IAction
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

        #region IAction interface

        string IAction.AnimationName => Definition.AnimationName;

        int IAction.AreaOfEffect => Definition.AreaOfEffect;

        ReadOnlyCollection<EffectDescriptor> IAction.EffectDescriptors => Definition.EffectDescriptors;

        int IAction.EnergyCost => Definition.EnergyCost;

        InPlaceEffectType IAction.InPlaceEffectType => Definition.InPlaceEffectType;

        ProjectileDescriptor? IAction.Projectile => Definition.Projectile;

        Sound? IAction.SoundStart => Definition.SoundStart;

        Sound? IAction.SoundTrigger => Definition.SoundTrigger;

        ActionKind IAction.ActionKind => Definition.ActionKind;


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
                    field = int.Clamp(value, 0, Definition.IsStackable || Definition.IsDepletable ? GameSettings.MaxItemAmount : 1);
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
            actor.Energy -= Definition.EnergyCost;

            if (Definition.IsDepletable)
            {
                Amount--;
                if (Amount <= 0)
                    Inventory.Remove(this);

                if (Definition.ConsumeVerb != LogVerb.None)
                    actor.Session.TextHUD.Log.Show(Definition.ConsumeVerb, Definition, true);
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
