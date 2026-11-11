namespace Adberration.Scripting
{
    // ElseStatement
    internal sealed class ElseStatement : Statement
    {
        // Constructor
        internal ElseStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.Else, source, body, 0)
        {
        }
    }
}
