using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// Config
    /// </summary>
    public abstract class Config
    {
        // Constructor
        protected Config(string name, IList<string> tags, ChanceTable lootTable)
        {
            this.Name = name;
            this.Tags = new(tags);
            this.LootTable = lootTable;
        }

        // LootTable
        public ChanceTable LootTable { get; }

        // Name
        public string Name { get; }

        // Tags
        public ReadOnlyCollection<string> Tags { get; }
    }
}
