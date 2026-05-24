using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // UseItemCommand
    // Arguments: {ItemName}
    [ForceAwait]
    internal sealed class UseItemCommand : NonAwaitableCommand
    {
        private readonly ItemDefinition definition;

        // Constructor
        internal UseItemCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 1)
        {
            definition = Script.AssertItemDefinition(Body.Clauses[0]);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;


            if (session.Player == null)
                return;

            if (Session.OutcomeTarget is not GameThing target)
                return;

            session.Player.PerformAction(definition, target);

            if (session.PlayerInventory.Find(definition.Name) is Item item)
                item.Consume();
        }
    }
}