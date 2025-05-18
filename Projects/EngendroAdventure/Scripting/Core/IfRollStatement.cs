using Engendro;

namespace EngendroAdventure.Scripting
{
    // IfRollStatement
    // Syntax: {DiceExpression} {== | != | > | < | <= | >=} {Integer}
    internal sealed class IfRollStatement : ConditionalStatement
    {
        private readonly DiceExpression diceExpression;
        private readonly ComparisonOperator comparisonOperator;

        // Constructor
        internal IfRollStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body)
        {
            diceExpression = Parser.ParseDiceExpression(this, body.Clauses[0]);
            comparisonOperator = Parser.ParseComparisonOperator(this, body.Clauses[1]);
            Parser.ParseInt32(this, 2);
        }

        // Evaluate
        public override bool Evaluate()
        {
            return EvaluateCore(comparisonOperator, diceExpression.Roll(), Parser.ParseInt32(this, 2));
        }
    }
}
