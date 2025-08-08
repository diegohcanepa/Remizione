using System;

namespace Adberration.Scripting
{
    // IfEntityTypeStatement
    // Arguments: {Entity} is {Type}
    internal sealed class IfEntityTypeStatement : SelectionStatement
    {
        private readonly Type entityType;

        // Constructor
        internal IfEntityTypeStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 3)
        {
            AssertEntity<Entity>(0);
            AssertKeyword(1, ScriptSyntax.IsOp);
            entityType = AssertEntityClass(body.Clauses[2]);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            var entity = AssertEntity<Entity>(0);
            if (entity == null)
            {
                return false;
            }
            else
            {
                return entityType.IsAssignableFrom(entity.GetType());
            }
        }
    }
}
