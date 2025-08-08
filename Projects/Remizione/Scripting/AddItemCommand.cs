using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AddItemCommand
    // Syntax: {Item} to {Actor} [#amount:Integer]
    internal sealed class AddItemCommand : NonAwaitableCommand
    {
        private readonly MetaItem? metaItem;

        // Constructor
        internal AddItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, AmountArg, EquipArg)
        {
            var itemName = Parser.ParseName(this, 0);
            AssertKeyword(1, "to");
            Parser.ParseEntity<Actor>(this, 2);
            Parser.ParseInt32Argument(this, AmountArg);

            metaItem = MetaItem.Find(itemName);
            if (metaItem == null)
                throw ScriptExceptionBuilder.InvalidValue(this, $"Item type'[{itemName}]'.");
        }

        // OnExecute
        protected override void OnExecute()
        {
            var actor = Parser.ParseEntity<Actor>(this, 2);
            if (actor == null || metaItem == null)
                return;

            var amount = Parser.ParseInt32Argument(this, AmountArg, 1);
            var container = actor.Inventory.GetContainer(metaItem.Category);
            if (container.Add(metaItem.Name, amount) is Item item && HasArg(EquipArg))
                item.Equip();
        }
    }
}
