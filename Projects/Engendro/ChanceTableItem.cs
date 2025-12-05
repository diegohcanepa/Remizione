namespace Engendro
{
    /// <summary>
    /// ChanceTableItem
    /// </summary>
    public sealed class ChanceTableItem
    {
        // Constructor
        public ChanceTableItem(string name, int amount, float weight, object? context = null)
        {
            this.Name = name;
            this.Amount = amount;
            this.Weight = weight;
            this.Context = context;
        }

        // Amount
        public int Amount { get; }

        // Context
        public object? Context { get; }

        // Name
        public string Name { get; }

        // Weight
        public float Weight { get; }
    }
}
