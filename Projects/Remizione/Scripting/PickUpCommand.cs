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

            if (target.ItemReward == null)
                return;

            target.ItemReward.PickupSound?.Play();

            var collectAmount = target.ItemRewardAmount;
            if (session.PlayerData.Inventory.Find(target.ItemReward.Name) is Item carriedItem)
                collectAmount = GameSettings.MaxItemAmount - carriedItem.Amount;

            target.ItemRewardAmount -= collectAmount;

            if (session.PlayerData.Inventory.Add(target.ItemReward, collectAmount) is Item item)
            {
                var startPos = ((target.Position - Session.Camera.Position) * Session.Camera.Zoom) + (new Vector2(240, 135) * 0.5f);
                session.HUD.InventoryMeter.AnimateAddItem(item, startPos);
            }

            if (session.Player != null)
                EffectDescriptor.Apply(target.ItemReward.EffectDescriptors, session.Player, null, EffectContext.Collect);

            if (target.ItemRewardAmount == 0)
            {
                target.ItemReward = null;
                target.Unparent();
            }
        }
    }
}
