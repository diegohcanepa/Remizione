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

            if (session.OutcomeTarget is not ILootContainer lootContainer)
                return;

            session.OutcomeTarget.Unparent();

            if (lootContainer.Loot == null)
                return;

            lootContainer.Loot.PickupSound?.Play();

            if (lootContainer.Loot.Behavior == ItemBehavior.Sack)
            {
                if (session.CurrentRun?.PlayerInventory.Add(lootContainer.Loot) is Item item)
                {
                    session.RunHUD?.InventoryMeter.Animate();

                    if (item.Definition.Image != null)
                        session.RunHUD?.Log.Show(item.DisplayName, item.Definition.Image);
                }
            }
            else if (lootContainer.Loot.Behavior != ItemBehavior.PlayerAction && session.Player != null)
            {
                EffectDescriptor.Apply(lootContainer.Loot.EffectDescriptors, session.Player, null, EffectContext.Collect);
            }

            lootContainer.Loot = null;
        }
    }
}
