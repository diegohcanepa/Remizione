using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // PlaceItemCommand
    // Arguments: {ItemName}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class PlaceItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal PlaceItemCommand(Script script, string source, StatementBody args)
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

            if (session.Room != null)
            {
                if (Inventory.FindInAll(Body.Clauses[0]) is Item item)
                {
                    var instance = new Firecracker(session, item, player.Position);
                    session.Room.Children.Add(instance);
                }
            }
        }
    }
}
