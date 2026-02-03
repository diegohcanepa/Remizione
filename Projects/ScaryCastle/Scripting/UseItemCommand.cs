using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName} [#target:GameThing]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1, TargetArg)
        {
            if (ItemDefinition.Find(Body.Clauses[0]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[0]}' is not defined.");

            Parser.ParseEntityArgument<GameThing>(this, TargetArg, null);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.Player is not Actor player)
                return;

            if (session.Inventory.Find(Body.Clauses[0]) is Item item)
            {
                if (HasArg(TargetArg))
                {
                    if (Parser.ParseEntityArgument<GameThing>(this, TargetArg, null) is GameThing target)
                        item.Use(player, target);
                }
                else
                {
                    item.Use(player, player);
                }
            }
        }
    }
}
