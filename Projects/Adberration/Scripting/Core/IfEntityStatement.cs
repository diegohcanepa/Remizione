using System.Linq;

namespace Adberration.Scripting
{
    // IfEntityStatement
    // Arguments: {Entity} {==|!=} {Entity1[,...]}
    internal sealed class IfEntityStatement : SelectionStatement
    {
        // Constructor
        internal IfEntityStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 3)
        {
            AssertEntity<Entity>(0);
            AssertKeyword(1, ScriptSyntax.EqualityOp, ScriptSyntax.InequalityOp, ScriptSyntax.InOp, ScriptSyntax.NotInOp);
            Parser.ParseEntities<Entity>(this, 2);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            var leftOperand = AssertEntity<Entity>(0);
            var rightOperand = Parser.ParseEntities<Entity>(this, 2);
            var op = Body.Clauses[1];

            if (op == ScriptSyntax.EqualityOp || op == ScriptSyntax.InOp)
            {
                return rightOperand.Contains(leftOperand);
            }
            else
            {
                return !rightOperand.Contains(leftOperand);
            }
        }
    }
}
