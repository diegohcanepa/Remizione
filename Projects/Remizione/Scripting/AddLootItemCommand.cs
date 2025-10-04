using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AddLootItemCommand
    // Arguments: {MetaItem} weight {float} [#amount:Integer]
    internal sealed class AddLootItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddLootItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, AmountArg)
        {
            if (string.IsNullOrWhiteSpace(BeginLootTableCommand.ActiveName))
                throw new ScriptException(script, "You need to call begin-loot-table first.");

            var itemName = body.Clauses[0];
            AssertKeyword(1, "weight");
            var weight = Parser.ParseFloat(this, 2);

            var lootTable = ChanceTable.Find(BeginLootTableCommand.ActiveName);
            lootTable ??= ChanceTable.Register(BeginLootTableCommand.ActiveName);

            var amount = Parser.ParseInt32Argument(this, AmountArg, 1);

            lootTable.Add(itemName, amount, weight);
        }
    }
}
