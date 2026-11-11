namespace Adberration.Scripting
{
    // ConditionalStatement
    // Arguments: {Entity}[.{PropertyName}] {== | != | >} {Entity}[.{PropertyName}]
    public abstract class ConditionalStatement : SelectionStatement
    {
        // Constructor
        internal ConditionalStatement(Script script, StatementType statementType, string source, StatementBody body)
            : base(script, statementType, source, body, 3)
        {
        }

        // EvaluateCore
        protected static bool EvaluateCore(ComparisonOperator op, int leftOperand, int rightOperand)
        {
            return op switch
            {
                // Inequality
                ComparisonOperator.Inequality => leftOperand != rightOperand,

                // Greater
                ComparisonOperator.GreaterThan => leftOperand > rightOperand,

                // GreaterThan
                ComparisonOperator.GreaterThanOrEqual => leftOperand >= rightOperand,

                // Less
                ComparisonOperator.LessThan => leftOperand < rightOperand,

                // LessThan
                ComparisonOperator.LessThanOrEqual => leftOperand <= rightOperand,

                // Equality
                ComparisonOperator.Equality => leftOperand == rightOperand,

                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
