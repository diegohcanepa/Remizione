using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AddItemCommand
    // Syntax: {Item} [#amount:Integer]
    internal sealed class AddItemCommand : NonAwaitableCommand
    {
        private readonly MetaItem? metaItem;

        // Constructor
        internal AddItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, AmountArg, EquipArg)
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
            if (session.PilgrimSack.Add(metaItem.Name, amount) is Item item && HasArg(EquipArg))
            {
                if (metaItem.IsEquipment)
                    session.PilgrimSack.Equip(item);
            }
        }
    }
}
