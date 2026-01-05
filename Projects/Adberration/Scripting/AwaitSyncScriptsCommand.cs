namespace Adberration.Scripting
{
    // AwaitSyncScriptsCommand
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitSyncScriptsCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitSyncScriptsCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting => Session.IsAwaiting;
    }
}
