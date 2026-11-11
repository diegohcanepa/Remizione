namespace Adberration.Scripting
{
    /// JumpStatement
    public abstract class JumpStatement : Statement
    {
        // Constructor
        protected JumpStatement(Script script, StatementType statementType, string source, StatementBody body, int clauseCount)
            : base(script, statementType, source, body, clauseCount)
        {
        }
    }
}
