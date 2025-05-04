using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorFatigueState
    /// </summary>
    public sealed class ActorFatigueState : ActorAnimatedState
    {
        private readonly FloatTween tween = new();

        // Constructor
        public ActorFatigueState(Actor owner)
            : base(owner, ActorStateNames.Fatigue, true)
        {
        }

        // Enter
        public override void Enter()
        {
            base.Enter();

            var newAnger = Owner.MaxAnger * .3f;

            if (Owner.AnimationPlayer.Animation != null)
                tween.Start(TweenStyle.Linear, Owner.Anger, newAnger, 2000);
            else
                Owner.Anger = newAnger;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            if (tween.IsRunning)
            {
                tween.Update(gameTime);
                Owner.Anger = tween.CurrentValue;
            }
        }
    }
}
