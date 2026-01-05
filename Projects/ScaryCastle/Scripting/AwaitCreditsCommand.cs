using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitCredits
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitCreditsCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitCreditsCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // IsAwaiting
        public override bool IsAwaiting => Session.Room is CreditsRoom room && room.IsShowingCredits;
    }
}
