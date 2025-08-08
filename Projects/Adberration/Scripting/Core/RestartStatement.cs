namespace Adberration.Scripting
{
    /// <summary>
    /// RestartStatement
    /// </summary>
    internal sealed class RestartStatement : JumpStatement
    {
        // Constructor
        internal RestartStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.Restart, source, body, 0)
        {
        }
    }
}
