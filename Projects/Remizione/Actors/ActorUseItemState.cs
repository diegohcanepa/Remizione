using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorUseItemState
    /// </summary>
    public sealed class ActorUseItemState : ActorAnimatedState
    {
        private bool soundPlayed;

        // Constructor
        public ActorUseItemState(Actor owner)
            : base(owner, ActorStateNames.UseItem, false)
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
            soundPlayed = false;
        }

        // Exit
        public override void Exit()
        {
            base.Exit();
            Owner.EndTurn();
            Item = null;
        }

        // Item
        public Item? Item { get; set; }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (!soundPlayed && Item != null && Owner.AnimationPlayer.Frame is SpriteFrame frame)
            {
                if (frame.IsEvent)
                {
                    Item.Use();

                    if (Item.MetaItem.Sound != null)
                        Owner.PlaySound(Item.MetaItem.Sound);

                    soundPlayed = true;
                }
            }
        }
    }
}
