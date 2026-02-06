using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // IfCanPickupLootStatement
    internal sealed class IfCanPickUpLootStatement : SelectionStatement
    {
        // Constructor
        internal IfCanPickUpLootStatement(Script script, string source, StatementBody args)
            : base(script, StatementType.If, source, args, 0)
        {
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            if (Session is not GameSession session)
                return false;

            if (session.OutcomeTarget is ILootConatiner<ItemDefinition> lootContainer && lootContainer.Loot != null)
            {
                var inventory = lootContainer.Loot.InventoryCategory == InventoryCategory.Common ? session.CommonInventory : session.SacredInventory;
                if (!inventory.HasSpace(lootContainer.Loot))
                {
                    session.HUD.Message.Show(MessageKind.InventoryFull);
                    return false;
                }
            }

            return true;
        }
    }
}
