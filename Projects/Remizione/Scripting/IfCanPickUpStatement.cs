using Adberration.Scripting;

namespace Remizione.Scripting
{
    // IfCanPickUpStatement
    internal sealed class IfCanPickUpStatement : SelectionStatement
    {
        // Constructor
        internal IfCanPickUpStatement(Script script, string source, StatementBody args)
            : base(script, StatementType.If, source, args, 1)
        {
            AssertEntity<GameThing>(0);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            if (Session is not GameSession session)
                return false;

            if (AssertEntity<GameThing>(0) is not { } target)
                return false;

            if (target.ItemReward != null)
            {
                if (!session.PlayerData.Inventory.CanAddItem(target.ItemReward))
                {
                    session.HUD?.Message.Show(MessageKind.InventoryFull);
                    return false;
                }
            }

            return true;
        }
    }
}
