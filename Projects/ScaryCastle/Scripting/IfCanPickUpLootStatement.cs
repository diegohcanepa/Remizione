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

            if (session.CurrentRun != null)
            {
                if (session.OutcomeTarget is ILootContainer<ItemDefinition> lootContainer && lootContainer.Loot != null)
                {
                    if (!session.CurrentRun.PlayerInventory.HasSpace(lootContainer.Loot))
                    {
                        session.RunHUD?.Message.Show(MessageKind.InventoryFull);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
