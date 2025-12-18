using Adberration.Scripting;

namespace ScaryCastle.Scripting
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
            (Session as GameSession)?.Inventory.Clear();
        }
    }
}
