namespace Engendro
{
    /// <summary>
    /// Achievement
    /// </summary>
    public sealed class Achievement : INamedObject
    {
        // Constructor
        internal Achievement(int index, string name)
        {
            CodeContract.NotEmpty(name, nameof(name));

            this.Index = index;
            this.Name = name;
        }

        // Index
        public int Index { get; }

        // Name
        public string Name { get; }

        // ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
