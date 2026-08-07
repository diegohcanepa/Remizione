using Adberration.Scripting;
using Engendro;
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
        #region Constructor

        // Constructor
        public Item(ItemContainer inventory, ItemDefinition definition)
        {
            this.Inventory = inventory;
            this.Definition = definition;
            this.Script = inventory.Session.ScriptLibrary.FindRoutine($"{Definition.Name}Outcome");
            this.Amount = definition.InitialAmount;
            this.DisplayName = Localization.GetItemName(definition);
            this.Description = Localization.GetItemDescription(definition);
            this.ShortDescription = EffectDescriptor.GetDescription(definition.EffectDescriptors);
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

        // Amount
        public int Amount
        {
            get;
            set
            {
                if (value != field)
                {
                    field = int.Clamp(value, 0, Definition.IsStackable || Definition.IsDepletable ? GameSettings.MaxItemAmount : 1);
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
            }
        }

        // Definition
        public ItemDefinition Definition { get; }

        // Description
        public string Description { get; }

        // DisplayName
        public string DisplayName { get; }

        // Index
        public int Index => Inventory.IndexOf(this);

        // Inventory
        public ItemContainer Inventory { get; }

        // MissChance
        public Ratio MissChance { get; }

        // Name
        public string Name => Definition.Name;

        // Remove
        public void Remove()
        {
            Inventory.Remove(this);
        }

        // Script
        public Script? Script { get; }

        // ShortDescription
        public string ShortDescription { get; }

        // ToString
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
