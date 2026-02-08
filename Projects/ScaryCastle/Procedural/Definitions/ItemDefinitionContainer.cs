using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaryCastle
{
    /// <summary>
    /// ItemDefinitionContainer
    /// </summary>
    public sealed class ItemDefinitionContainer : DataContainer<ItemDefinition>
    {
        // Constructor
        public ItemDefinitionContainer(Func<JsonElement, ItemDefinition> onCreate)
            : base(onCreate)
        {
        }

        // GetItems
        public List<ItemDefinition> GetItems(ItemCategory category)
        {
            var result = new List<ItemDefinition>();

            for (var i = 0; i < All.Count; i++)
            {
                if (All[i].Category == category)
                    result.Add(All[i]);
            }

            return result;
        }

        // GetItems
        public List<ItemDefinition> GetItems(Realm realm)
        {
            var result = new List<ItemDefinition>();

            for (var i = 0; i < All.Count; i++)
            {
                if (All[i].Realm == realm)
                    result.Add(All[i]);
            }

            return result;
        }
    }
}
