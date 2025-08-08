namespace Adberration.Scripting
{
    // IfCounterStatement
    // Arguments: {Counter} {==|!=|<|<=|>|>=} {Integer}
    internal sealed class IfCounterStatement : SelectionStatement
    {
        // Constructor
        internal IfCounterStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 3)
        {
            AssertCounter(0);
            Parser.ParseComparisonOperator(this, 1);
            Parser.ParseInt32(this, 2);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            var counter = AssertCounter(0);
            var op = Parser.ParseComparisonOperator(this, 1);
            var value = Parser.ParseInt32(this, 2);

            return op switch
            {
                ComparisonOperator.Equality => counter.Value == value,

                ComparisonOperator.Inequality => counter.Value != value,

                ComparisonOperator.GreaterThan => counter.Value > value,

                ComparisonOperator.GreaterThanOrEqual => counter.Value >= value,

                ComparisonOperator.LessThan => counter.Value < value,

                ComparisonOperator.LessThanOrEqual => counter.Value <= value,

                _ => false,
            };
        }
    }
}
