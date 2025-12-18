using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AddItemCommand
    // Syntax: {Item} [#amount:Integer]
    internal sealed class AddItemCommand : NonAwaitableCommand
    {
        private readonly MetaItem? metaItem;

        // Constructor
        internal AddItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, AmountArg)
        {
            var itemName = Parser.ParseName(this, 0);
            Parser.ParseInt32Argument(this, AmountArg);

            metaItem = MetaItem.Find(itemName);
            if (metaItem == null)
                throw ScriptExceptionBuilder.InvalidValue(this, $"Item type'[{itemName}]'.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (metaItem == null)
                return;

            if (Session is not GameSession session)
                return;

            var amount = Parser.ParseInt32Argument(this, AmountArg, 1);
            session.Inventory.Add(metaItem.Name, amount);
        }
    }
}
