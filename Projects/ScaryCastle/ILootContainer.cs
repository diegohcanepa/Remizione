namespace ScaryCastle
{
    /// <summary>
    /// ILootContainer
    /// </summary>
    public interface ILootContainer<T> where T : Definition
    {
        T? Loot { get; set; }
    }
}
