using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // CastLightningCommand
    // Arguments: {ItemName}
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

            if (session.OutcomeTarget is not GameThing target)
                return;

            if (player.Room != null)
            {
                if (session.PlayerInventory.Find(Body.Clauses[0]) is Item item)
                {
                    var lightning = new LightningInvocation(target, item);
                    player.Room.Children.Add(lightning);
                }
            }
        }
    }
}
