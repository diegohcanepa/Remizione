using Engendro;
using Engendro.Audio;
using EngendroAdventure.Scripting;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// MetaItem
    /// </summary>
    public sealed class MetaItem
    {
        private static readonly Dictionary<string, MetaItem> items = [];

        #region Constructor

        // Constructor
        public MetaItem(string name, ItemKind kind, ItemAction action, bool passive, DiceExpression? baseDamage, Stat modifier, int bonus, Vector2 knockback, int maximum, DiceExpression? hp, DiceExpression? faith, int range, int durability, int degradationInterval, UpgradeHardness upgradeHardness, Sound? sound, int useInterval, Script? creationRoutine)
        {
            CodeContract.NotEmpty(name, nameof(name));

            if (items.ContainsKey(name))
                throw new InvalidOperationException($"The meta item '{name}' already exists.");
            else
                items[name] = this;

            if (passive && action != ItemAction.None)
                throw new InvalidOperationException("A passive item cannot define an action.");

            if (action == ItemAction.Create && creationRoutine == null)
                throw new InvalidOperationException("A creatable item must specify a creation rountine.");

            if (action != ItemAction.Create && creationRoutine != null)
                throw new InvalidOperationException("A creation routine is only for creatable items.");

            this.Name = name;
            this.Kind = kind;
            this.Bonus = bonus;
            this.Action = action;
            this.CreationRoutine = creationRoutine;
            this.Passive = passive;
            this.BaseDamage = baseDamage;
            this.Durability = durability;
            this.DegradationInterval = degradationInterval;
            this.Knockback = knockback;
            this.Maximum = Math.Max(1, maximum);
            this.Faith = faith;
            this.HP = hp;
            this.Range = range;
            this.Modifier = modifier;
            this.LocalizedDescription = TextRepository.GetValue($"Item.{Name}.Description");
            this.LocalizedName = TextRepository.GetValue($"Item.{Name}.Name");
            this.UpgradeHardness = upgradeHardness;
            this.Sound = sound;
            this.UseInterval = useInterval;
        }

        #endregion

        #region Static members

        // Find
        public static MetaItem? Find(string name) => items.TryGetValue(name, out var result) ? result : null;

        #endregion

        // Action
        public ItemAction Action { get; }

        // BaseDamage
        public DiceExpression? BaseDamage { get; }

        // Bonus
        public int Bonus { get; }

        // CreationRoutine
        public Script? CreationRoutine { get; }

        // DegradationInterval
        public int DegradationInterval { get; }

        // Durability
        public int Durability { get; set; }

        // Faith
        public DiceExpression? Faith { get; }

        // HP
        public DiceExpression? HP { get; }

        // IsStackable
        public bool IsStackable => Maximum > 1;

        // Kind
        public ItemKind Kind { get; }

        // Knockback
        public Vector2 Knockback { get; }

        // LocalizedDescription
        public string LocalizedDescription { get; }

        // LocalizedName
        public string LocalizedName { get; }

        // Maximum
        public int Maximum { get; }

        // Modifier
        public Stat Modifier { get; }

        // Name
        public string Name { get; }

        // Passive
        public bool Passive { get; }

        // Range
        public int Range { get; }

        // Sound
        public Sound? Sound { get; }

        // ToString
        public override string ToString() => Name;

        // UpgradeHardness
        public UpgradeHardness UpgradeHardness { get; }

        // UseInterval
        public int UseInterval { get; }
    }
}
