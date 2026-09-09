using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione.Scripting
{
    // PickUp
    internal sealed class PickUpCommand : NonAwaitableCommand
    {
        // Constructor
        internal PickUpCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<GameThing>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<GameThing>(0) is not GameThing target)
                return;

            target.Unparent();

            if (target.ItemReward == null)
                return;

            target.ItemReward.PickupSound?.Play();

            if (session.PlayerData.Inventory.Add(target.ItemReward) is Item item)
            {
                var startPos = ((target.Position - Session.Camera.Position) * Session.Camera.Zoom) + (new Vector2(240, 135) * 0.5f);
                session.HUD.InventoryMeter.AnimateAddItem(item, startPos);
            }

            if (session.Player != null)
                EffectDescriptor.Apply(target.ItemReward.EffectDescriptors, session.Player, null, EffectContext.Collect);

            target.ItemReward = null;
        }
    }
}
