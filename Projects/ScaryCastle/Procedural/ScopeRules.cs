using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ScopeRules
    /// </summary>
    public sealed class ScopeRules
    {
        // Constructor
        public ScopeRules(IList<string> allowPools, IList<string> denyPools, IList<string> allowTags, IList<string> denyTags)
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

        // FromJson
        public static ScopeRules FromJson(JsonElement element)
        {
            var allowPools = Tags.FromJson(element, "allowPools");
            var denyPools = Tags.FromJson(element, "denyPools");
            var allowTags = Tags.FromJson(element, "allowTags");
            var denyTags = Tags.FromJson(element, "denyTags");

            return new ScopeRules(allowPools, denyPools, allowTags, denyTags);
        }
    }
}
