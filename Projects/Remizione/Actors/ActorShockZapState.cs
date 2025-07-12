using Engendro;
using Engendro.Audio;
using EngendroAdventure;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// ActorShockZapState
    /// </summary>
    public sealed class ActorShockZapState : ActorAnimatedState
    {
        private int cooldown;
        private readonly FloatTween xTween = new();
        private readonly FloatTween yTween = new();

        // Constructor
        public ActorShockZapState(Actor owner)
            : base(owner, ActorStateNames.ShockZap, true)
        {
        }

        // CheckTransitions
        public override string? CheckTransitions()
        {
            if (cooldown <= 0)
                return ActorStateNames.Stand;
            else
                return base.CheckTransitions();
        }

        // Enter
        public override void Enter()
        {
            base.Enter();
            cooldown = 300;

            yTween.Start(TweenStyle.CubicOut, Owner.Y, Owner.Y-3, 150, 2);
            xTween.Start(TweenStyle.CubicOut, Owner.X, Owner.X + (Owner.Direction == FacingDirection.Right ? -15 : 15), 300);

            Owner.PlaySound(SoundNames.ShockZap);
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.Milliseconds;
            xTween.Update(gameTime);
            yTween.Update(gameTime);

            //Owner.X = xTween.CurrentValue;
            //Owner.Y = yTween.CurrentValue;
        }
    }
}
