namespace ScaryCastle
{
    /// <summary>
    /// RunModifier
    /// </summary>
    public sealed class RunModifier
    {
        // Constructor
        public RunModifier(RunModifierKind modifierKind, RunModifierScope scope)
        {
            this.Kind = modifierKind;
            this.Scope = scope;
        }

        // Kind
        public RunModifierKind Kind { get; }

        // Scope
        public RunModifierScope Scope { get; }
    }
}
