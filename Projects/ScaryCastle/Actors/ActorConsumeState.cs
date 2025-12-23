using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// ActorConsumeState
    /// </summary>
    public sealed class ActorConsumeState : ActorAnimatedState
    {
        private bool soundPlayed;

        // Constructor
        public ActorConsumeState(Actor owner)
            : base(owner, ActorStateNames.Consume, false)
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
                    if (Item.MetaItem.Effect.Sound is Sound sound)
                        Owner.PlaySound(sound);

                    soundPlayed = true;
                }
            }
        }
    }
}
