using System.Collections.Generic;

namespace ScaryCastle
{
    /// <summary>
    /// TraitCollection
    /// </summary>
    public sealed class TraitCollection : VersionedCollection<TraitDescriptor>
    {
        private readonly Dictionary<TraitDescriptor, float> valuesByTrait = [];

        #region Protected members

        // ClearItems
        protected override void ClearItems()
        {
            base.ClearItems();
            valuesByTrait.Clear();
        }

        // InsertItem
        protected override void InsertItem(int index, TraitDescriptor item)
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
            if (TraitDescriptor.Data.Find(traitType.ToString()) is TraitDescriptor traitDescriptor)
                Add(traitDescriptor);
        }

        // GetTotalTraitValue
        public float GetTotalTraitValue(TraitType traitType)
        {
            if (TraitDescriptor.Data.Find(traitType.ToString()) is TraitDescriptor traitDescriptor)
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
