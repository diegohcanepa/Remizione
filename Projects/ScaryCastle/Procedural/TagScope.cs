using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
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

        // FromJson
        public static TagScope FromJson(JsonElement element)
        {
            var allowPools = Tags.FromJson(element, "allowPools");
            var denyPools = Tags.FromJson(element, "denyPools");
            var allowTags = Tags.FromJson(element, "allowTags");
            var denyTags = Tags.FromJson(element, "denyTags");

            return new TagScope(allowPools, denyPools, allowTags, denyTags);
        }

        // Test
        public static bool Test(TagScope scope, Tags pools, Tags tags)
        {
            // DenyPools
            if (scope.DenyPools.Count > 0)
            {
                if (scope.DenyPools.Intersects(pools))
                    return false;
            }

            // DenyTags
            if (scope.DenyTags.Count > 0)
            {
                if (scope.DenyTags.Intersects(tags))
                    return false;
            }

            // AllowPools (si existe, requiere intersección)
            if (scope.AllowPools.Count > 0)
            {
                if (!scope.AllowPools.Intersects(pools))
                    return false;
            }
            else
            {
                // AllowTags VACÍO -> aceptar todo (equivalente a "any")
                if (scope.AllowTags.Count > 0)
                {
                    // si hay al menos una tag en allow, requerimos intersección
                    if (!scope.AllowTags.Intersects(tags))
                        return false;
                }

                // si AllowTags está vacío o es null, no filtramos por tags (aceptamos)
            }

            return true;
        }
    }
}
