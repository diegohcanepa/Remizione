namespace EngendroAdventure.Scripting
{
    /// <summary>
    /// Counter
    /// </summary>
    public sealed class Counter
    {
        // Constructor
        public Counter(string name, int value, bool persistent)
        {
            NameValidator.CheckName(name);
            this.Name = name;
            this.Value = value;
            this.Persistent = persistent;
        }

        // Name
        public string Name { get; }

        // Persistent
        public bool Persistent { get; }

        // Value
        public int Value { get; set; }
    }
}
