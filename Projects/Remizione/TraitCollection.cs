using Engendro;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// TraitCollection
    /// </summary>
    public sealed class TraitCollection : VersionedCollection<TraitDefinition>
    {
        private readonly Dictionary<TraitDefinition, float> valuesByTrait = [];

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            valuesByTrait.Clear();
        }

        // InsertItem
        protected override void InsertItem(int index, TraitDefinition item)
        {
            base.InsertItem(index, item);

            if (valuesByTrait.ContainsKey(item))
            {
                valuesByTrait[item] += item.Value;
            }
            else
            {
                valuesByTrait[item] = item.Value;
            }
        }

        // RemoveItem
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            valuesByTrait.Remove(item);
        }

        #endregion

        // Add
        public void Add(TraitType traitType)
        {
            if (GameData.Traits.Find(traitType.ToString()) is TraitDefinition traitDescriptor)
                Add(traitDescriptor);
        }

        // GetTotalTraitValue
        public float GetTotalTraitValue(TraitType traitType)
        {
            if (GameData.Traits.Find(traitType.ToString()) is TraitDefinition traitDescriptor)
            {
                return valuesByTrait.TryGetValue(traitDescriptor, out var value) ? value : 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
