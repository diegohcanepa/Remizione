using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // AwaitPickUpCommand
    // Arguments: {Actor} {PickupItem} #sound:Name
    [ForceAwait]
    internal sealed class AwaitPickUpCommand : AwaitableCommand
    {
        private Actor? actor;

        // Constructor
        internal AwaitPickUpCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2)
        {
            AssertEntity<Actor>(0);
            AssertEntity<Pickup>(1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            // Actor
            actor = AssertEntity<Actor>(0);
            if (actor == null)
                return;

            if (AssertEntity<Pickup>(1) is not Pickup pickupItem)
                return;

            var metaItem = MetaItem.Find(pickupItem.StaticName);

            if (!actor.Session.InventoryEnabled)
            {
                actor.Session.HUD.Message.Show(HUDMessageKind.NoInventoryBag, true);
                return;
            }

            // Stackable item already in inventory
            if (metaItem != null && metaItem.IsStackable && actor.Inventory.GetItem(metaItem.Name) is Item item)
            {
                if (item.IsStackFull)
                {
                    actor.Session.HUD.Message.Show(HUDMessageKind.EnoughOfThat, true);
                    return;
                }
            }

            // Inventory is full
            if (actor.Inventory.Items.Count == actor.InventorySize)
            {
                actor.Session.HUD.Message.Show(HUDMessageKind.InventoryFull, true);
                return;
            }

            actor.PickUp(pickupItem, metaItem);
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            actor = null;
        }

        // IsAwaiting
        public override bool IsAwaiting => actor != null && actor.IsPickingUp;
    }
}
