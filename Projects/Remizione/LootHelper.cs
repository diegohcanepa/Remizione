/*
using System;
using System.Collections.Generic;

namespace Remizione
{
    public static class LootHelper
    {
        // HasAny
        public static bool HasAny(LootTag[] itemTags, LootTag[] contextTags)
        {
            for (int i = 0; i < itemTags.Length; i++)
                for (int j = 0; j < contextTags.Length; j++)
                    if (itemTags[i] == contextTags[j])
                        return true;
            return false;
        }

        // HasAll
        public static bool HasAll(LootTag[] contextTags, LootTag[] requiredTags)
        {
            for (int i = 0; i < requiredTags.Length; i++)
            {
                bool found = false;
                for (int j = 0; j < contextTags.Length; j++)
                {
                    if (requiredTags[i] == contextTags[j]) { found = true; break; }
                }
                if (!found) return false;
            }
            return true;
        }

        // HasExcluded
        public static bool HasExcluded(LootTag[] contextTags, LootTag[] excludedTags)
        {
            for (int i = 0; i < excludedTags.Length; i++)
                for (int j = 0; j < contextTags.Length; j++)
                    if (excludedTags[i] == contextTags[j])
                        return true;
            return false;
        }

        // IsEligible
        public static bool IsEligible(LootContext context, ItemDefinition item)
        {
            if (!HasAny(item.Tags, context.Tags)) return false;
            if (!HasAll(context.Tags, item.RequiredTags)) return false;
            if (HasExcluded(context.Tags, item.ExcludedTags)) return false;
            if (item.MinDepth >= 0 && context.Depth < item.MinDepth) return false;
            if (item.MaxDepth >= 0 && context.Depth > item.MaxDepth) return false;
            return true;
        }

        // GetFinalWeight
        public static float GetFinalWeight(MetaItem metaItem)
        {
            float qm = metaItem.Quality switch
            {
                0 => 0.85f,
                1 => 1.00f,
                2 => 1.25f,
                3 => 1.60f,
                4 => 2.10f,
                _ => 1f
            };
            return metaItem.BaseWeight * qm;
        }

        // RollWeighted
        public static MetaItem? RollWeighted(Random rng, List<MetaItem> items, List<float> weights)
        {
            float total = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                total += weights[i];
            }
            
            if (total <= 0)
                return null;

            float r = (float)(rng.NextDouble() * total);
            float acc = 0f;

            for (int i = 0; i < items.Count; i++)
            {
                acc += weights[i];
                if (r <= acc)
                    return items[i];
            }

            return items[^1];
        }
    }
}
*/