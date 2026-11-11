using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ReadOnlyPlaceholderCollection
    /// </summary>
    public sealed class ReadOnlyPlaceholderCollection : ReadOnlyCollection<Placeholder>
    {
        // Constructor
        public ReadOnlyPlaceholderCollection(IList<Placeholder> list)
            : base(list)
        {
        }

        // AllowsTag
        public bool AllowsTag(Tag tag)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].AllowTags.Contains(tag))
                    return true;
            }

            return false;
        }

        // GetPlaceholdersByTag
        public IList<Placeholder> GetPlaceholdersByTag(Tag tag)
        {
            var result = new List<Placeholder>();

            for (var i = 0; i < Count; i++)
            {
                if (this[i].AllowTags.Contains(tag))
                    result.Add(this[i]);
            }

            return result.Count > 0 ? result : Array.Empty<Placeholder>();
        }
    }
}
