namespace EngendroAdventure.Scripting
{
    // IfRandomNumberStatement
    // Syntax: {Name} {== | != | > | < | <= | >=} {Integer}
    internal sealed class IfRandomNumberStatement : ConditionalStatement
    {
        // Constructor
        internal IfRandomNumberStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body)
        {
            Parser.ParseName(this, 0);

            // Comparison operator
            Parser.ParseComparisonOperator(this, body.Clauses[1]);

            Parser.ParseInt32(this, 2);
        }

        // Evaluate
        public override bool Evaluate()
        {
            var leftOp = Session.GetRandomNumber(Parser.ParseName(this, 0));
            var op = Parser.ParseComparisonOperator(this, Body.Clauses[1]);
            return EvaluateCore(op, leftOp, Parser.ParseInt32(this, 2));
        }
    }
}
