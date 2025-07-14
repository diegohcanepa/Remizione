using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorThrowItemState
    /// </summary>
    public sealed class ActorThrowItemState : ActorAnimatedState
    {
        private bool itemUsed;

        // Constructor
        public ActorThrowItemState(Actor owner)
            : base(owner, AnimationNames.ThrowItem, false)
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
            itemUsed = false;
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
            if (!itemUsed && Item != null && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.IsEvent)
                {
                    if (Owner.Session.ObjectPools.GetThrowable(Item.Name) is Throwable throwable)
                    {
                        if (Owner.WhooshSound != null)
                            Owner.PlaySound(Owner.WhooshSound);

                        throwable.Launch(Item);
                    }

                    itemUsed = true;
                }
            }
        }
    }
}