namespace Engendro
{
    /// <summary>
    /// ChanceTableItem
    /// </summary>
    public sealed class ChanceTableItem
    {
        // Constructor
        public ChanceTableItem(string name, int amount, float weight, object? tag = null)
        {
            this.Name = name;
            this.Amount = amount;
            this.Weight = weight;
            this.Tag = tag;
        }

        // Amount
        public int Amount { get; }

        // Name
        public string Name { get; }

        // Tag
        public object? Tag { get; }

        // Weight
        public float Weight { get; }
    }
}
