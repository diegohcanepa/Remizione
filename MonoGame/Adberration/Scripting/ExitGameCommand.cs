using Engendro;

namespace Adberration.Scripting
{
    // ExitGameCommand
    internal sealed class ExitGameCommand : NonAwaitableCommand
    {
        // Constructor
        internal ExitGameCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            EngendroGame.Instance.Exit();
        }
    }
}
