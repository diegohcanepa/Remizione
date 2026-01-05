using Engendro.Input;

namespace Adberration.Scripting
{
    // SuspendInputCommand
    // Arguments: {Duration:Integer}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class SuspendInputCommand : NonAwaitableCommand
    {
        // Constructor
        internal SuspendInputCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseInt32(this, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            InputManager.Suspend(Parser.ParseInt32(this, 0));
        }
    }
}
