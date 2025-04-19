using Engendro;

namespace EngendroAdventure.Scripting
{
    // IfRollStatement
    // Syntax: {DiceName} {== | != | > | < | <= | >=} {Integer}
    internal sealed class IfRollStatement : ConditionalStatement
    {
        // Constructor
        internal IfRollStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body)
        {
            Parser.ParseEnum<DiceName>(this, 0);

            // Comparison operator
            Parser.ParseComparisonOperator(this, body.Clauses[1]);

            Parser.ParseInt32(this, 2);
        }

        // Evaluate
        public override bool Evaluate()
        {
            var dice = DiceBag.GetDice(Parser.ParseEnum<DiceName>(this, 0));
            var op = Parser.ParseComparisonOperator(this, Body.Clauses[1]);
            return EvaluateCore(op, dice.Roll(), Parser.ParseInt32(this, 2));
        }
    }
}
