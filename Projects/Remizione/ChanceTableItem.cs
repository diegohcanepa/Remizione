namespace Remizione
{
    /// <summary>
    /// ChanceTableItem
    /// </summary>
    public sealed class ChanceTableItem
    {
        // Constructor
        public ChanceTableItem(string name, int amount, float weight)
        {
            this.Name = name;
            this.Amount = amount;
            this.Weight = weight;
        }

        // Amount
        public int Amount { get; }

        // Name
        public string Name { get; }

        // Weight
        public float Weight { get; }
    }
}
