namespace Adberration.Scripting
{
    // IfNotNullStatement
    // Arguments: {Entity}
    internal sealed class IfNotNullStatement : SelectionStatement
    {
        // Constructor
        internal IfNotNullStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 1)
        {
            AssertEntity<Entity>(0);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            return AssertEntity<Entity>(0) != null;
        }
    }
}
