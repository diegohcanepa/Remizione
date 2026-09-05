using Adberration.Scripting;

namespace Remizione.Scripting
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
                if (session.OutcomeTarget is ILootContainer lootContainer && lootContainer.Loot != null)
                {
                    if (!session.PlayerData.Inventory.HasSpace(lootContainer.Loot))
                    {
                        session.HUD?.Message.Show(MessageKind.InventoryFull);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
