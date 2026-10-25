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
            ResetBreakState();
        }

        #endregion

        #region IAction interface

        string IAction.AnimationName => Definition.AnimationName;

        int IAction.AreaOfEffect => Definition.AreaOfEffect;

        ReadOnlyCollection<EffectDescriptor> IAction.EffectDescriptors => Definition.EffectDescriptors;

        ImpactEffectType IAction.ImpactEffectType => Definition.ImpactEffectType;

        Sound? IAction.SoundStart => Definition.SoundStart;

        Sound? IAction.SoundTrigger => Definition.SoundTrigger;

        ActionKind IAction.ActionKind => Definition.ActionKind;


        #endregion

        #region Private members

        // TryDegrade
        public bool TryDegrade()
        {
            if (!Definition.HasBreakChance)
                return false;

            Uses++;

            // Protegemos durante la ventana de usos seguros
            if (Uses <= Definition.BreakSafeUses)
                return false;

            // Evaluamos rotura contra el Ratio acumulado
            if (BreakChance.Roll())
            {
                Amount--;
                ResetBreakState();
                return true;
            }

            // Sobrevivió: incrementamos la probabilidad de rotura para el próximo uso
            BreakChance += Definition.BreakChanceIncrement;
            return false;
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

            // 1. Si el ítem tiene mecánica de desgaste/rotura, evaluamos degradación
            if (Definition.HasBreakChance)
            {
                TryDegrade();
            }
            // 2. Si no tiene probabilidad de rotura pero es un consumible estándar (ej. poción)
            else if (Definition.IsDepletable)
            {
                Amount--;
            }
        }

        // BreakChance
        public Ratio BreakChance { get; set; }

        // Definition
        public ItemDefinition Definition { get; }

        // HPCost
        public int HPCost => Definition.HPCost;

        // Index
        public int Index => Inventory.IndexOf(this);

        // Inventory
        public ItemContainer Inventory { get; }

        // IsStackFull
        public bool IsStackFull => Definition.IsStackable || Definition.IsDepletable ? Amount == GameSettings.MaxItemAmount : Amount == 1;

        // MissChance
        public Ratio MissChance { get; }

        // Name
        public string Name => Definition.Name;

        // Perish
        public bool Perish()
        {
            if (!Definition.IsPerishable || PerishState == ItemPerishState.Rotten)
                return false;

            PerishState++;

            return true;
        }

        // PerishState
        public ItemPerishState PerishState { get; set; }

        // ProjectileImageName
        public string ProjectileImageName => Definition.ProjectileImageName;

        // Remove
        public void Remove()
        {
            Inventory.Remove(this);
        }

        // ResetBreakState
        public void ResetBreakState()
        {
            Uses = 0;
            BreakChance = Definition.BreakBaseChance;
        }

        // Script
        public Script? Script { get; }

        // ToString
        public override string ToString()
        {
            return Definition.Label;
        }

        // Uses
        public int Uses { get; set; }
    }
}
