using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ConsumeItemCommand
    // Syntax: {Item}
    internal sealed class ConsumeItemCommand : NonAwaitableCommand
    {
        private readonly ItemDefinition definition;

        // Constructor
        internal ConsumeItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var itemName = Parser.ParseName(this, 0);
            definition = ItemDefinition.Data.Get(itemName);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Player == null)
                return;

            if (session.PlayerInventory.Find(definition.Name) is Item item)
                item.Consume(session.Player);
        }
    }
}
