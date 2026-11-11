namespace Adberration.Scripting
{
    // EndifStatement
    internal sealed class EndifStatement : Statement
    {
        // Constructor
        internal EndifStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.Endif, source, body, 0)
        {
        }
    }
}
