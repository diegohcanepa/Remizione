namespace Engendro.Collections
{
    /// <summary>
    /// ChanceTableItem
    /// </summary>
    public sealed class ChanceTableItem
    {
        // Constructor
        public ChanceTableItem(string name, float weight, object? context = null)
        {
            CodeContract.NotEmpty(name, nameof(name));
            CodeContract.GreaterThanZero(weight, nameof(weight));

            this.Name = name;
            this.Weight = weight;
            this.Context = context;
        }

        // Context
        public object? Context { get; }

        // Name
        public string Name { get; }

        // Weight
        public float Weight { get; }
    }
}
