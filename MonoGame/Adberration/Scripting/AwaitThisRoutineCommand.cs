namespace Adberration.Scripting
{
    // AwaitThisRoutineCommand
    [ForceAwait]
    internal sealed class AwaitThisRountineCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitThisRountineCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Session.AwaitScript(Script);
        }
    }
}
