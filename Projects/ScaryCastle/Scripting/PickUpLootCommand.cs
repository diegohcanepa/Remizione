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

            if (session.OutcomeTarget is ILootContainer<ItemDefinition> lootProvider)
            {
                session.OutcomeTarget.Unparent();

                if (lootProvider.Loot != null)
                {
                    lootProvider.Loot.PickupSound?.Play();

                    if (lootProvider.Loot.Behavior == ItemBehavior.Common)
                    {
                        session.PlayerInventory.Add(lootProvider.Loot);
                        session.StatusHUD.InventoryMeter.Animate();
                        session.TextHUD.Log.Show(LogVerb.Found, lootProvider.Loot);
                    }
                    else if (lootProvider.Loot.Behavior != ItemBehavior.PlayerAction && session.Player != null)
                    {
                        EffectDescriptor.Apply(lootProvider.Loot.EffectDescriptors, session.Player, null, EffectContext.Collect);
                    }

                    lootProvider.Loot = null;
                }
            }
        }
    }
}
