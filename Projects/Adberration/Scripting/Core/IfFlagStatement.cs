namespace Adberration.Scripting
{
    // IfFlagStatement
    // Arguments: {Flag[,Flag...]}
    internal sealed class IfFlagStatement : SelectionStatement
    {
        private readonly FlagCondition condition;

        // Constructor
        internal IfFlagStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 1)
        {
            var condition = Parser.ParseFlagCondition(this, body.Clauses[0]);
            if (condition == null)
            {
                throw new ScriptException(this, "Missing flag expression.");
            }
            else
            {
                this.condition = condition;
            }
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            return condition.Evaluate();
        }
    }
}
