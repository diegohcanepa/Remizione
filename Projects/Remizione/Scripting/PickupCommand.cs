using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // PickupCommand
    // Arguments: {Actor} {PickupItem}
    internal sealed class PickupCommand : NonAwaitableCommand
    {
        // Constructor
        internal PickupCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2)
        {
            AssertEntity<Actor>(0);
            AssertEntity<PickupItem>(1);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Actor>(0) is not Actor actor)
                return;

            if (AssertEntity<PickupItem>(1) is not PickupItem pickupItem)
                return;

            if (MetaItem.Find(pickupItem.StaticName) is not MetaItem metaItem)
                return;

            // Stackable item already in inventory
            if (metaItem.IsStackable && actor.Inventory.GetItem(metaItem.Name) is Item item)
            {
                if (item.IsStackFull)
                {
                    actor.Session.HUD.Log.Show(LogMessage.EnoughOfThat, true);
                    return;
                }
            }

            // Inventory is full
            if (actor.Inventory.IsFull)
            {
                actor.Session.HUD.Log.Show(LogMessage.InventoryFull, true);
                return;
            }

            actor.Inventory.Add(metaItem, 1);
            pickupItem.Unparent();
            actor.Session.HUD.Log.Show(LogVerb.PickedUp, metaItem.LocalizedName);
        }
    }
}
