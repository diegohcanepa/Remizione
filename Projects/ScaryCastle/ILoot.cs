namespace ScaryCastle
{
    /// <summary>
    /// ILoot
    /// </summary>
    public interface ILoot<T> where T : Definition
    {
        T? Loot { get; set; }
    }
}
