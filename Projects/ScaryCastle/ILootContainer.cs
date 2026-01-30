namespace ScaryCastle
{
    /// <summary>
    /// ILootContainer
    /// </summary>
    public interface ILootConatiner<T> where T : Definition
    {
        T? Loot { get; set; }
    }
}
