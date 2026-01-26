using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // PickUpLootCommand
    internal sealed class PickUpLootCommand : NonAwaitableCommand
    {
        // Constructor
        internal PickUpLootCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (session.OutcomeTarget is ILootConatiner<ItemDefinition> itemLootContainer)
            {
                if (itemLootContainer.Loot != null)
                {
                    itemLootContainer.Loot.PickupSound?.Play();
                    session.Inventory.Add(itemLootContainer.Loot);
                    itemLootContainer.Loot = null;
                }
            }
        }
    }
}
