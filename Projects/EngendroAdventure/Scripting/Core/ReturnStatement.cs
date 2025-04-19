namespace EngendroAdventure.Scripting
{
    /// ReturnStatement
    internal sealed class ReturnStatement : JumpStatement
    {
        // Constructor
        internal ReturnStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.Return, source, body, 0)
        {
        }
    }
}
