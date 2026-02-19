using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseSacredItemCommand
    // Arguments: {ItemName}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class UseSacredItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal UseSacredItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            Script.AssertItemDefinition(Body.Clauses[0]);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.Player is not Actor player)
                return;

            if (Inventory.FindInAll(Body.Clauses[0]) is Item item)
            {
            }
        }
    }
}
