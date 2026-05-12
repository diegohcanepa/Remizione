using System.Linq;

namespace Adberration.Scripting
{
    // IfEntityStatement
    // Arguments: {Entity} {==|!=|in|not-in} {Entity[,...]}
    internal sealed class IfEntityStatement : SelectionStatement
    {
        // Constructor
        internal IfEntityStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 3)
        {
            AssertEntity<Entity>(0);
            AssertKeyword(1, ScriptSyntax.EqualityOp, ScriptSyntax.InequalityOp, ScriptSyntax.InOp, ScriptSyntax.NotInOp);

            var op = Body.Clauses[1];

            if (op == ScriptSyntax.EqualityOp || op == ScriptSyntax.InequalityOp)
                Parser.ParseEntities<Entity>(this, 2);
            else
                AssertEntity<Entity>(2);
        }

        // EvaluateEquality
        private bool EvaluateEquality()
        {
            var leftOperand = AssertEntity<Entity>(0);
            var rightOperand = AssertEntity<Entity>(2);
            return rightOperand == leftOperand;
        }

        // EvaluateInequality
        private bool EvaluateInequality()
        {
            var leftOperand = AssertEntity<Entity>(0);
            var rightOperand = AssertEntity<Entity>(2);
            return rightOperand != leftOperand;
        }

        // EvaluateExclusion
        private bool EvaluateExclusion()
        {
            var leftOperand = AssertEntity<Entity>(0);
            var rightOperand = Parser.ParseEntities<Entity>(this, 2);
            return !rightOperand.Contains(leftOperand);
        }

        // EvaluateInclusion
        private bool EvaluateInclusion()
        {
            var leftOperand = AssertEntity<Entity>(0);
            var rightOperand = Parser.ParseEntities<Entity>(this, 2);
            return rightOperand.Contains(leftOperand);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            var op = Body.Clauses[1];

            if (op == ScriptSyntax.EqualityOp)
            {
                return EvaluateEquality();
            }
            else if (op == ScriptSyntax.InequalityOp)
            {
                return EvaluateInequality();
            }
            else if (op == ScriptSyntax.InOp)
            {
                return EvaluateInclusion();
            }
            else
            {
                return EvaluateExclusion();
            }
        }
    }
}
