using System.Collections.ObjectModel;

namespace Remizione
{
    /// <summary>
    /// ILootContext
    /// </summary>
    public interface ILootContext
    {
        ReadOnlyCollection<LootTag> LootTags { get; }
    }
}