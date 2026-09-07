using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using System.Collections.ObjectModel;

namespace Remizione
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
        }

        #endregion

        #region IAction interface

        string IAction.AnimationName => Definition.AnimationName;

        int IAction.AreaOfEffect => Definition.AreaOfEffect;

        ReadOnlyCollection<EffectDescriptor> IAction.EffectDescriptors => Definition.EffectDescriptors;

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
                    Inventory.Version++;
                }
            }
        }

        // Consume
        public void Consume(Actor actor)
        {
            actor.ApplyAction(this);

            if (Definition.IsDepletable)
            {
                Amount--;
                if (Amount <= 0)
                    Inventory.Remove(this);
            }
        }

        // Definition
        public ItemDefinition Definition { get; }

        // HPCost
        public int HPCost => Definition.HPCost;

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

        // ToString
        public override string ToString()
        {
            return Definition.DisplayName;
        }
    }
}
