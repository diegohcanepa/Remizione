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
                    lootContainer.Loot.PickupSound?.Play();
                    session.Inventory.Add(lootContainer.Loot);
                    session.HUD.Log.Show(LogVerb.Found, lootContainer.Loot);
                    lootContainer.Loot = null;
                }
            }
        }
    }
}
