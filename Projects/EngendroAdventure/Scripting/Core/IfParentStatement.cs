namespace EngendroAdventure.Scripting
{
    // IfParentStatement
    // Arguments: {Child:Entity} {==|!=} {Parent:Entity}
    internal sealed class IfParentStatement : SelectionStatement
    {
        // Constructor
        internal IfParentStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 3)
        {
            AssertEntity<Entity>(0);
            AssertKeyword(1, ScriptSyntax.EqualityOp, ScriptSyntax.InequalityOp);
            AssertEntity<Entity>(2);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            var child = AssertEntity<Entity>(0);
            var parent = AssertEntity<Entity>(2);

            if (child == null)
            {
                return false;
            }
            else
            {
                return Body.Clauses[1] == ScriptSyntax.EqualityOp ? child.Parent == parent : child.Parent != parent;
            }
        }
    }
}
