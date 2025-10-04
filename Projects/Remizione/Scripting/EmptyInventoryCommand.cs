using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EmptyInventoryCommand
    // Syntax: {Actor}
    internal sealed class EmptyInventoryCommand : NonAwaitableCommand
    {
        // Constructor
        internal EmptyInventoryCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseEntity<Actor>(this, 0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Parser.ParseEntity<Actor>(this, 0) is Actor actor)
                actor.Inventory.Clear();
        }
    }
}
