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
        private int currentUses;
        private Ratio currentBreakChance;

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

            currentUses++;

            // Protegemos durante la ventana de usos seguros
            if (currentUses <= Definition.BreakSafeUses)
                return false;

            // Evaluamos rotura contra el Ratio acumulado
            if (currentBreakChance.Roll())
            {
                Amount--;
                ResetBreakState();
                return true;
            }

            // Sobrevivió: incrementamos la probabilidad de rotura para el próximo uso
            currentBreakChance = currentBreakChance + Definition.BreakChanceIncrement;
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

        // DecayState
        public ItemDecayState DecayState { get; set; }

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
            currentUses = 0;
            currentBreakChance = Definition.BreakBaseChance;
        }

        // Script
        public Script? Script { get; }

        // ToString
        public override string ToString()
        {
            return Definition.Label;
        }
    }
}
