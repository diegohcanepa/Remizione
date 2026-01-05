namespace Adberration.Scripting
{
    // StopFollowingCommand
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class StopFollowingCommand : NonAwaitableCommand
    {
        // Constructor
        internal StopFollowingCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            Session.Camera.StopFollowing();
        }
    }
}
