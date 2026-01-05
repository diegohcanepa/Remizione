namespace Adberration.Scripting
{
    // StopShakingCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class StopShakingCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopShakingCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Session.Camera.StopShaking();
        }
    }
}
