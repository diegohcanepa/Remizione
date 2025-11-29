using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// TagScope
    /// </summary>
    public sealed class TagScope
    {
        // Constructor
        public TagScope(IList<string> allowPools, IList<string> denyPools, IList<string> allowTags, IList<string> denyTags)
        {
            this.AllowPools = new(allowPools);
            this.DenyPools = new(denyPools);

            this.AllowTags = new(allowTags);
            this.DenyTags = new(denyTags);
        }

        // AllowPools
        public Tags AllowPools { get; }

        // AllowTags
        public Tags AllowTags { get; }

        // DenyPools
        public Tags DenyPools { get; }

        // DenyTags
        public Tags DenyTags { get; }
    }
}
