using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ScopeRules
    /// </summary>
    public sealed class ScopeRules
    {
        // Constructor
        public ScopeRules(IList<string> allowPools, IList<string> denyPools, IList<string> allowTags, IList<string> denyTags, int maxPerRoom = -1)
        {
            this.AllowPools = new(allowPools);
            this.DenyPools = new(denyPools);

            this.AllowTags = new(allowTags);
            this.DenyTags = new(denyTags);

            this.MaxPerRoom = maxPerRoom;
        }

        // AllowPools
        public Tags AllowPools { get; }

        // AllowTags
        public Tags AllowTags { get; }

        // DenyPools
        public Tags DenyPools { get; }

        // DenyTags
        public Tags DenyTags { get; }

        // MaxPerRoom
        public int MaxPerRoom { get; }
    }
}
