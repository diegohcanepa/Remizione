using Adberration.Scripting;

namespace Remizione.Scripting
{
    // PlaceItemCommand
    // Arguments: {ItemName}
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
                if (session.CurrentRun?.PlayerInventory.Find(Body.Clauses[0]) is Item item)
                {
                    // TODO: Check
                    //var instance = new Firecracker(session, item, player.Position);
                    //session.Room.Children.Add(instance);
                }
            }
        }
    }
}
