using Engendro.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace Remizione
{
    /// <summary>
    /// TagScope
    /// </summary>
    public sealed class TagScope
    {
        // Constructor
        public TagScope(IList<Tag> allowPools, IList<Tag> denyPools, IList<Tag> allowTags, IList<Tag> denyTags)
        {
            this.AllowPools = new(allowPools);
            this.DenyPools = new(denyPools);
            this.AllowTags = new(allowTags);
            this.DenyTags = new(denyTags);
        }

        // AllowPools
        public ReadOnlyEnumSet<Tag> AllowPools { get; }

        // AllowTags
        public ReadOnlyEnumSet<Tag> AllowTags { get; }

        // DenyPools
        public ReadOnlyEnumSet<Tag> DenyPools { get; }

        // DenyTags
        public ReadOnlyEnumSet<Tag> DenyTags { get; }

        // FromJson
        public static TagScope FromJson(JsonElement element)
        {
            var allowPools = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "allowPools");
            var denyPools = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "denyPools");
            var allowTags = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "allowTags");
            var denyTags = ReadOnlyEnumSet<Tag>.FromJsonOrEmpty(element, "denyTags");

            return new TagScope(allowPools, denyPools, allowTags, denyTags);
        }

        // Test
        public static bool Test(TagScope scope, ReadOnlyEnumSet<Tag> pools, ReadOnlyEnumSet<Tag> tags)
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
