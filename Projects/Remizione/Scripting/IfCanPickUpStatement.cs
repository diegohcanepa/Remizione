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
                // Check stack
                if (session.PlayerData.Inventory.Find(target.ItemReward.Name) is Item item)
                {
                    if (item.IsStackFull)
                    {
                        session.HUD?.Message.Show(MessageKind.StackFull);
                        return false;
                    }
                }

                // Check for free inventory slot
                else if (session.PlayerData.Inventory.IsFull)
                {
                    session.HUD?.Message.Show(MessageKind.InventoryFull);
                    return false;
                }
            }

            return true;
        }
    }
}
