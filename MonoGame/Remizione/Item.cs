using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
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
        public Item(ItemContainer container, ItemDefinition definition)
        {
            this.Container = container;
            this.Definition = definition;
            this.Script = container.Session.ScriptLibrary.FindRoutine($"{Definition.Name}Outcome");
            this.Amount = definition.InitialAmount;
            ResetBreakState();
        }

        #endregion

        #region IAction interface

        ActionKind IAction.ActionKind => Definition.ActionKind;

        bool IAction.AllowSelfAction => Definition.AllowSelfAction;

        string IAction.AnimationName => Definition.AnimationName;

        int IAction.AreaOfEffect => Definition.AreaOfEffect;

        ReadOnlyCollection<EffectDescriptor> IAction.EffectDescriptors => Definition.EffectDescriptors;

        ImpactEffectType IAction.ImpactEffectType => Definition.ImpactEffectType;

        bool IAction.Spawn => Definition.Spawn;

        Sound? IAction.SoundStart => Definition.SoundStart;

        Sound? IAction.SoundTrigger => Definition.SoundTrigger;

        #endregion

        #region Private members

        // TryDegrade
        private bool TryDegrade()
        {
            if (!Definition.HasBreakChance)
                return false;

            Uses++;

            // Safe uses
            if (HasSafeUses)
                return false;

            // Break chances
            if (BreakChance.Roll())
            {
                Amount--;
                ResetBreakState();
                return true;
            }

            // Incremente break chances
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
                        Container.Remove(this);
                    Container.Version++;
                }
            }
        }

        // ApplyUsage
        public ApplyUsageOutcome ApplyUsage(Actor? actor)
        {
            actor?.ApplyAction(this);

            // 1. Si el ítem tiene mecánica de desgaste/rotura, evaluamos degradación
            if (Definition.HasBreakChance)
            {
                if (TryDegrade())
                    return ApplyUsageOutcome.Broken;
            }

            // 2. Si no tiene probabilidad de rotura pero es un consumible estándar (ej. poción)
            else if (Definition.IsDepletable)
            {
                Amount--;
                return ApplyUsageOutcome.Consumed;
            }

            return ApplyUsageOutcome.None;
        }

        // BreakChance
        public Ratio BreakChance { get; set; }

        // Container
        public ItemContainer Container { get; }

        // Definition
        public ItemDefinition Definition { get; }

        // ElapsedPerishTime
        public float ElapsedPerishTimer { get; set; }

        // HasBreakSafeUses
        public bool HasSafeUses => Uses <= Definition.BreakSafeUses;

        // HPCost
        public int HPCost => Definition.HPCost;

        // Index
        public int Index => Container.IndexOf(this);

        // IsStackFull
        public bool IsStackFull => Definition.IsStackable || Definition.IsDepletable ? Amount == GameSettings.MaxItemAmount : Amount == 1;

        // MissChance
        public Ratio MissChance { get; }

        // Name
        public string Name => Definition.Name;

        // Perish
        public bool Perish()
        {
            if (Definition.ItemKind != ItemKind.Organic || PerishState == ItemPerishState.Rotten)
                return false;

            PerishState++;

            Container.Version++;

            return true;
        }

        // PerishState
        public ItemPerishState PerishState { get; set; }

        // ProjectileImageName
        public string ProjectileImageName => Definition.ProjectileImageName;

        // Remove
        public void Remove()
        {
            Container.Remove(this);
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

        // Update
        public void Update(GameTime gameTime)
        {
            if (Definition.PerishInterval > 0)
            {
                ElapsedPerishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ElapsedPerishTimer >= Definition.PerishInterval)
                {
                    ElapsedPerishTimer = 0;
                    Perish();
                }
            }
        }

        // Uses
        public int Uses { get; set; }

        // Verb
        public Verb Verb { get; set; }
    }
}
