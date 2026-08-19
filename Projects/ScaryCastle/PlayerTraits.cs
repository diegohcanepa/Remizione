using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ScaryCastle
{
    /// <summary>
    /// PlayerTraits
    /// </summary>
    public sealed class PlayerTraits
    {
        private readonly List<TraitDescriptor> traitList = [];
        private readonly Dictionary<TraitType, float> valuesByTrait = [];

        // Constructor
        public PlayerTraits()
        {
            this.Traits = traitList.AsReadOnly();
        }

        // Add
        public void Add(TraitType traitType)
        {
            if (TraitDescriptor.Data.Find(traitType.ToString()) is { } descriptor)
            {
                if (valuesByTrait.ContainsKey(traitType))
                {
                    valuesByTrait[traitType] += descriptor.Value;
                }
                else
                {
                    traitList.Add(descriptor);
                    valuesByTrait[traitType] = descriptor.Value;
                }

                ContentVersion++;
            }
        }

        // Clear
        public void Clear()
        {
            traitList.Clear();
            ContentVersion = 0;
        }

        // ContentVersion
        public int ContentVersion { get; private set; }

        // GetTotalTraitValue
        public float GetTotalTraitValue(TraitType traitType)
        {
            return valuesByTrait.TryGetValue(traitType, out var value) ? value : 0;
        }

        // Remove
        public void Remove(TraitType traitType)
        {
            if (TraitDescriptor.Data.Find(traitType.ToString()) is { } descriptor)
            {
                traitList.Remove(descriptor);
                ContentVersion++;
            }
        }

        // Traits
        public ReadOnlyCollection<TraitDescriptor> Traits { get; }
    }
}
