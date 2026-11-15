using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// BodyDropItemState
    /// </summary>
    public sealed class BodyDropItemState : BodyAnimatedState
    {
        private bool eventDone;

        // Constructor
        public BodyDropItemState()
            : base(AnimationNames.PickUp, false)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            eventDone = false;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Item = null;
        }

        // Item
        public Item? Item { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (Item != null && !eventDone && Owner.AnimationPlayer.Frame?.IsTrigger == true)
            {
                eventDone = true;
                Item.Remove();
                if (Owner.Room != null)
                {
                    Owner.PlaySound(SoundNames.ItemDrop);
                    ItemOrb.Drop(Item.Definition, Item.Amount, Owner.Room, Owner.Position + new Vector2(0,3));
                }
                return;
            }

            if (!Owner.AnimationPlayer.IsPlaying)
                Machine.ChangeState<BodyStandState>();
        }
    }
}