using Engendro;
using Microsoft.Xna.Framework;

namespace Adberration.Scripting
{
    // ColorTweenCommand
    // Arguments: {Entity} {TweenStyle} to {Color} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer]
    internal sealed class ColorTweenCommand : AwaitableCommand
    {
        private int duration;

        // Constructor
        internal ColorTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 6, BouncesArg, BounceDelayArg, DecimalsArg)
        {
            AssertEntity<Entity>(0);
            Parser.ParseEnum<TweenStyle>(this, 1);
            AssertKeyword(2, "to");
            Parser.ParseColor(this, 3);
            AssertKeyword(4, "duration");
            Parser.ParseInt32(this, 5);
            Parser.ParseInt32Argument(this, BouncesArg);
            Parser.ParseInt32Argument(this, BounceDelayArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var entity = AssertEntity<Entity>(0);
            if (entity == null)
            {
                return;
            }

            var style = Parser.ParseEnum<TweenStyle>(this, 1);
            var endValue = Parser.ParseColor(this, 3);
            duration = Parser.ParseInt32(this, 5);
            var bounces = Parser.ParseInt32Argument(this, BouncesArg);
            var bounceDelay = Parser.ParseInt32Argument(this, BounceDelayArg);

            ColorTween tween = new() { BounceDelay = bounceDelay };
            tween.Start(style, entity.Color, endValue, duration, bounces);

            entity.Tweens.ColorTween = tween;
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            if (duration >= 0)
            {
                duration -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        // IsAwaiting
        public override bool IsAwaiting => duration >= 0;
    }
}
