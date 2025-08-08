namespace Adberration.Scripting
{
    /// <summary>
    /// Flag
    /// </summary>
    public sealed class Flag
    {
        // Constructor
        internal Flag(string name, bool value, bool persistent)
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

        // SetValueReferenceCount
        public int SetValueReferenceCount { get; internal set; }

        // Value
        public bool Value { get; set; }

        // ToString
        public override string ToString() => $"{Name}={Value}";
    }
}
