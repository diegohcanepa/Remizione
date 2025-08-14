using Adberration.Scripting;

namespace Remizione.Scripting
{
    // UseItemCommand
    // Arguments: {Actor} {ItemName}
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2)
        {
            AssertEntity<Actor>(0);

            if (MetaItem.Find(Body.Clauses[1]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[1]}' is not defined.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            // Actor
            if (AssertEntity<Actor>(0) is not Actor actor)
                return;

            if (actor.Inventory.Find(Body.Clauses[1]) is Item item)
                item.Use();
        }
    }
}
