using Engendro;
using Microsoft.Xna.Framework;

namespace Adberration.Scripting
{
    // Vector2TweenCommand
    // Arguments: {Entity} {TweenStyle} to {Vector2} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative]
    internal abstract class Vector2TweenCommand : AwaitableCommand
    {
        private Vector2Tween? tween;

        // Constructor
        protected Vector2TweenCommand(Script script, string source, StatementBody body, params string[] supportedFlags)
            : base(script, source, body, 6, supportedFlags.Concatenate([BouncesArg, BounceDelayArg, DecimalsArg, LoopedArg, RelativeArg]))
        {
            AssertEntity<Entity>(0);
            Parser.ParseEnum<TweenStyle>(this, 1);
            AssertKeyword(2, "to");
            Parser.ParseVector2(this, 3);
            AssertKeyword(4, "duration");
            Parser.ParseInt32(this, 5);
            Parser.ParseInt32Argument(this, BouncesArg);
            Parser.ParseInt32Argument(this, BounceDelayArg);
        }

        // GetEndValue
        protected abstract Vector2 GetEndValue(Entity entity, Vector2 value);

        // GetStartValue
        protected abstract Vector2 GetStartValue(Entity entity);

        // OnExecute
        protected override void OnExecute()
        {
            var entity = AssertEntity<Entity>(0);
            if (entity == null)
            {
                return;
            }

            var style = Parser.ParseEnum<TweenStyle>(this, 1);
            var endValue = Parser.ParseVector2(this, 3);
            endValue = GetEndValue(entity, endValue);

            var duration = Parser.ParseInt32(this, 5);
            var bounces = HasArg(LoopedArg) ? -1 : Parser.ParseInt32Argument(this, BouncesArg);
            var bounceDelay = Parser.ParseInt32Argument(this, BounceDelayArg);

            tween = new Vector2Tween() { BounceDelay = bounceDelay };
            tween.Start(style, GetStartValue(entity), endValue, duration, bounces);

            SetTween(entity, tween);
        }

        // SetTween
        protected abstract void SetTween(Entity entity, Vector2Tween tween);

        // IsAwaiting
        public override bool IsAwaiting() => tween != null && tween.IsRunning;
    }
}
