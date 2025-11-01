using Adberration.Scripting;

namespace Remizione.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName}
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            if (MetaItem.Find(Body.Clauses[0]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[0]}' is not defined.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.Player == null)
                return;

            if (session.PilgrimSack.Find(Body.Clauses[0]) is Item item)
                item.Use(session.Player);
        }
    }
}
