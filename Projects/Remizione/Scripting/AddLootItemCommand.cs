using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // AddLootItemCommand
    // Arguments: {ItemName} {DropChance}
    internal sealed class AddLootItemCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddLootItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            var thing = AssertEntityNotNull<GameThing>(Script.EntityName);
            var itemName = Parser.ParseEnum<ItemName>(this, 0);
            var dropChance = Parser.ParseFloat(this, 1);

            var lootTable = LootTable.Find(thing.StaticName);
            lootTable ??= LootTable.Register(thing.StaticName);

            lootTable.Add(itemName, dropChance);
        }
    }
}
