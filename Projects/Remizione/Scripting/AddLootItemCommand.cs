using Adberration.Scripting;
using System;

namespace Remizione.Scripting
{
    // AddLootItemCommand
    // Arguments: {MetaItem} weight {float}
    internal sealed class AddLootItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddLootItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            if (string.IsNullOrWhiteSpace(BeginLootTableCommand.ActiveName))
                throw new ScriptException(script, "You need to call begin-loot-table first.");

            var itemName = body.Clauses[0];

            if (!Enum.IsDefined(typeof(ItemCategory), itemName))
            {
                if (itemName != ChanceTable.Nothing && MetaItem.Find(itemName) == null)
                    throw new ScriptException(script, $"MetaItem '{itemName}' not found.");
            }

            AssertKeyword(1, "weight");
            var weight = Parser.ParseFloat(this, 2);

            var lootTable = ChanceTable.Find(BeginLootTableCommand.ActiveName);
            lootTable ??= ChanceTable.Register(BeginLootTableCommand.ActiveName);
            lootTable.Add(itemName, 1, weight);
        }
    }
}
