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

            if (session.OutcomeTarget is ILoot<ItemDefinition> loot)
            {
                if (loot.Loot != null)
                {
                    loot.Loot.PickupSound?.Play();
                    session.Inventory.Add(loot.Loot);
                    session.HUD.InventoryMeter.Animate();
                    session.HUD.Log.Show(LogVerb.Found, loot.Loot);
                    loot.Loot = null;
                }
            }
        }
    }
}
