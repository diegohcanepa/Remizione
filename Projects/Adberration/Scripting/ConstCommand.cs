namespace Adberration.Scripting
{
    // ConstantCommand
    // Arguments: %{Name} = {Value}
    internal sealed class ConstCommand : NonAwaitableCommand
    {
        // Constructor
        internal ConstCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            AssertConstantName(0);
            AssertKeyword(1, ScriptSyntax.AssignmentOp);
            Session.ScriptEnvironment.DeclareConstant(Body.Clauses[0], Body.Clauses[2]);
        }
    }
}
