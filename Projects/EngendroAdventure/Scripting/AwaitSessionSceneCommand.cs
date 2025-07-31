namespace EngendroAdventure.Scripting
{
    // AwaitSessionSceneCommand
    [ForceAwait]
    internal sealed class AwaitSessionSceneCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitSessionSceneCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting => !Session.IsCurrentScene;
    }
}
