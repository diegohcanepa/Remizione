namespace EngendroAdventure.Scripting
{
    // AwaitOutcomeCompletionCommand
    [ForceAwait]
    internal sealed class AwaitOutcomeCompletionCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitOutcomeCompletionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting => Session.IsOutcomeInProgress;
    }
}
