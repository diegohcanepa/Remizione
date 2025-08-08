namespace Adberration.Scripting
{
    // IfRoutineRunningStatement
    // Arguments: {Routine} {==|!=} {Boolean}
    internal sealed class IfRoutineRunningStatement : SelectionStatement
    {
        // Constructor
        internal IfRoutineRunningStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 3)
        {
            AssertRoutine(0);
            AssertKeyword(1, ScriptSyntax.EqualityOp, ScriptSyntax.InequalityOp);
            Parser.ParseBoolean(this, 2);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            var routine = AssertRoutine(0);
            if (routine == null)
            {
                return false;
            }

            var value = Parser.ParseBoolean(this, 2);
            var isExecuting = Session.ScriptProcessor.IsExecutingScript(routine);

            return Body.Clauses[1] == ScriptSyntax.EqualityOp ? isExecuting == value : isExecuting != value;
        }
    }
}
