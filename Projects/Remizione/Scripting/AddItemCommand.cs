using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // AddItemCommand
    // Syntax: {ItemName} to {Actor} [#amount:Integer]
    internal sealed class AddItemCommand : NonAwaitableCommand
    {
        private readonly MetaItem? metaItem;

        // Constructor
        internal AddItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, AmountArg)
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
            actor.Inventory.Add(metaItem.Name, amount);
        }
    }
}
