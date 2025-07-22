using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorPickUpState
    /// </summary>
    public sealed class ActorPickUpState : ActorAnimatedState
    {
        private bool itemPickedUp;
        private MetaItem? metaItem;
        private Pickup? pickup;

        // Constructor
        public ActorPickUpState(Actor owner)
            : base(owner, ActorStateNames.PickUp, false)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!Owner.AnimationPlayer.IsPlaying)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            itemPickedUp = false;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            metaItem = null;
            pickup = null;
        }

        // Prepare
        public void Prepare(Pickup pickup, MetaItem? metaItem)
        {
            this.pickup = pickup;
            this.metaItem = metaItem;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (itemPickedUp || pickup == null)
                return;

            if (Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.IsEvent)
                {
                    if (metaItem != null)
                    {
                        Owner.Inventory.GetContainer(metaItem.Category).Add(metaItem, 1);
                        Owner.Session.HUD.Log.Show(LogVerb.PickedUp, metaItem.LocalizedName, metaItem.Image);
                    }

                    if (pickup.PickUpSound is Sound sound)
                        Owner.PlaySound(sound);

                    pickup.Unparent();
                    itemPickedUp = true;
                }
            }
        }
    }
}
