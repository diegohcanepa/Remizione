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

        /*
        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (!tween.IsRunning)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }
        */

        // Enter
        public override void Enter()
        {
            base.Enter();

            Owner.ShowMessage("@Messages.Fatigue");

            var newAnger = Owner.MaxAnger * .5f;

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
