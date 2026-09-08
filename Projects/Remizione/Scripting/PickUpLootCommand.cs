using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione.Scripting
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

            if (lootContainer.Loot.Behavior == ItemBehavior.Loot)
            {
                if (session.PlayerData.Inventory.Add(lootContainer.Loot) is Item item)
                {
                    var startPos = ((session.OutcomeTarget.Position - Session.Camera.Position) * Session.Camera.Zoom) + (new Vector2(240, 135) * 0.5f);
                    session.HUD.InventoryMeter.AnimateAddItem(item, startPos);
                }
            }

            if (lootContainer.Loot.Behavior != ItemBehavior.PlayerAction && session.Player != null)
                EffectDescriptor.Apply(lootContainer.Loot.EffectDescriptors, session.Player, null, EffectContext.Collect);

            lootContainer.Loot = null;
        }
    }
}
