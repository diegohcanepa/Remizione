using System;

namespace EngendroAdventure.Scripting
{
    // CallMethodCommand
    // Arguments: {Entity.}{MethodName}
    internal sealed class CallMethodCommand : NonAwaitableCommand
    {
        #region Constructor

        // Constructor
        internal CallMethodCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            if (!body.Clauses[0].EndsWith("()", StringComparison.Ordinal))
            {
                throw new ScriptException(this, "The name of a method must end with '()'.");
            }

            Parser.ParseMethodExpression(this, body.Clauses[0], true);
        }

        #endregion

        // OnExecute
        protected override void OnExecute()
        {
            if (Parser.ParseMethodExpression(this, Body.Clauses[0], true) is MethodExpression expr && expr.Instance != null && expr.Method != null)
            {
                expr.Method.Invoke(expr.Instance);
            }
        }
    }
}
