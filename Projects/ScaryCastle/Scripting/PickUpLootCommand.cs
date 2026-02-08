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

            if (session.OutcomeTarget is ILootConatiner<ItemDefinition> lootContainer)
            {
                if (lootContainer.Loot != null)
                {
                    var inventory = lootContainer.Loot.InventoryCategory == InventoryCategory.Common ? session.CommonInventory : session.SacredInventory;
                    lootContainer.Loot.PickupSound?.Play();
                    inventory.Add(lootContainer.Loot);
                    session.HUD.Log.Show(LogVerb.Found, lootContainer.Loot);
                    lootContainer.Loot = null;
                }
            }
        }
    }
}
