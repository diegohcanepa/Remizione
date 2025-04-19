namespace EngendroAdventure.Scripting
{
    // StopFollowingCommand
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
