using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ConfigScopeRule
    /// </summary>
    public sealed class ConfigScopeRule
    {
        // Constructor
        public ConfigScopeRule(IList<string> allowPools, IList<string> denyPools, IList<string> allowTags, IList<string> denyTags)
        {
            this.AllowPools = new(allowPools);
            this.DenyPools = new(denyPools);

            this.AllowTags = new(allowTags);
            this.DenyTags = new(denyTags);
        }

        // AllowPools
        public ReadOnlyCollection<string> AllowPools { get; }

        // AllowTags
        public ReadOnlyCollection<string> AllowTags { get; }

        // DenyPools
        public ReadOnlyCollection<string> DenyPools { get; }

        // DenyTags
        public ReadOnlyCollection<string> DenyTags { get; }
    }
}
