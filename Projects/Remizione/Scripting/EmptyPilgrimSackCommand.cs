using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EmptyPilgrimSackCommand
    internal sealed class EmptyPilgrimSackCommand : NonAwaitableCommand
    {
        // Constructor
        internal EmptyPilgrimSackCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseEntity<Actor>(this, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            (Session as GameSession)?.PilgrimSack.Clear();
        }
    }
}
