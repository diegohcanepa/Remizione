namespace Adberration.Scripting
{
    /// <summary>
    /// SelectionStatement
    /// </summary>
    public abstract class SelectionStatement : Statement
    {
        // Constructor
        protected SelectionStatement(Script script, StatementType statementType, string source, StatementBody body, int clauseCount)
            : base(script, statementType, source, body, clauseCount)
        {
        }

        // Evaluate
        public abstract bool Evaluate();
    }
}
