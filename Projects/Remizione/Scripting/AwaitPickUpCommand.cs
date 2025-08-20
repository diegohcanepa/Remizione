using Adberration.Scripting;

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
            if (AssertEntity<Actor>(0) is not Actor actor)
                return;

            // Pickup
            if (AssertEntity<Pickup>(1) is not Pickup pickup)
                return;

            // MetaItem
            if (MetaItem.Find(pickup.ItemName) is not MetaItem metaItem)
                return;

            var inventory = actor.Inventory.GetContainer(metaItem.Category);

            // Stackable item already in inventory
            if (metaItem.IsStackable && inventory.Find(metaItem.Name) is Item item)
            {
                if (item.IsStackFull)
                {
                    actor.Session.HUD.Message.Show(HUDMessageKind.EnoughOfThat);
                    return;
                }
            }

            // Inventory is full
            if (inventory.Count == inventory.Size)
            {
                actor.Session.HUD.Message.Show(HUDMessageKind.InventoryFull);
                return;
            }

            actor.PickUp(pickup, metaItem);
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
