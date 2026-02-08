using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            if (ItemDefinition.Definitions.Find(Body.Clauses[0]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[0]}' is not defined.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.Player is not Actor player)
                return;

            if (Inventory.FindInAll(Body.Clauses[0]) is Item item && session.OutcomeTarget is GameThing target)
                item.Use(player, target);
        }
    }
}
