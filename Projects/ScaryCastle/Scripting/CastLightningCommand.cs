using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // CastLightningCommand
    // Arguments: {ItemName}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class CastLightningCommand : NonAwaitableCommand
    {
        // Constructor
        internal CastLightningCommand(Script script, string source, StatementBody args)
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

            if (player.Room != null)
            {
                if (session.Inventory.Find(Body.Clauses[0]) is Item item)
                {
                    var lightning = new LightningRite(session, item);
                    player.Room.Children.Add(lightning);
                }
            }
        }
    }
}
