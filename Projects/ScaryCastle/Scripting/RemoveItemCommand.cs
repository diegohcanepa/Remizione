using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // RemoveItemCommand
    // Syntax: {Item}
    internal sealed class RemoveItemCommand : NonAwaitableCommand
    {
        private readonly ItemDefinition definition;

        // Constructor
        internal RemoveItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var itemName = Parser.ParseName(this, 0);
            definition = ItemDefinition.Definitions.Get(itemName);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (definition == null)
                return;

            if (Session is not GameSession session)
                return;

            var amount = Parser.ParseInt32Argument(this, AmountArg, 1);

            session.Inventory.Remove(definition.Name);
        }
    }
}
