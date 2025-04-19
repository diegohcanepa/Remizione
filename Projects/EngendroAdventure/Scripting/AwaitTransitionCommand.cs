using Engendro;

namespace EngendroAdventure.Scripting
{
    // AwaitTransitionCommand
    [ForceAwait]
    internal sealed class AwaitTransitionCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitTransitionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting => TransitionManager.CurrentTransition.IsRunning;
    }
}
