namespace Adberration.Scripting
{
    /// <summary>
    /// FlagExpression
    /// </summary>
    public sealed class FlagExpression
    {
        private readonly Flag flag;

        // Constructor
        public FlagExpression(Flag flag, bool negate)
        {
            this.flag = flag;
            this.Negate = negate;
        }

        // FlagName
        public string FlagName => flag.Name;

        // FlagValue
        public bool FlagValue => flag.Value;

        // Negate
        public bool Negate { get; }

        // ToString
        public override string ToString() => Negate ? "!" + FlagName : FlagName;
    }
}