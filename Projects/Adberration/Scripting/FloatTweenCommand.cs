using Engendro;

namespace Adberration.Scripting
{
    // FloatTweenCommand
    // Arguments: {Entity} {TweenStyle} to {float} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative] [#start-delay:Integer]
    public abstract class FloatTweenCommand : AwaitableCommand
    {
        private FloatTween? tween;

        // Constructor
        protected FloatTweenCommand(Script script, string source, StatementBody body, params string[] supportedFlags)
            : base(script, source, body, 6, supportedFlags.Concatenate([BouncesArg, BounceDelayArg, DecimalsArg, LoopedArg, RelativeArg, StartDelayArg]))
        {
            AssertEntity<Entity>(0);
            Parser.ParseEnum<TweenStyle>(this, 1);
            AssertKeyword(2, "to");
            Parser.ParseFloat(this, 3);
            AssertKeyword(4, "duration");
            Parser.ParseInt32(this, 5);
            Parser.ParseInt32Argument(this, BouncesArg);
            Parser.ParseInt32Argument(this, BounceDelayArg);
            Parser.ParseInt32Argument(this, StartDelayArg);
        }

        // GetEndValue
        protected abstract float GetEndValue(Entity entity, float value);

        // GetStartValue
        protected abstract float GetStartValue(Entity entity);

        // OnExecute
        protected override void OnExecute()
        {
            var entity = AssertEntity<Entity>(0);
            if (entity == null)
            {
                return;
            }

            var style = Parser.ParseEnum<TweenStyle>(this, 1);

            var endValue = Parser.ParseFloat(this, 3);
            endValue = GetEndValue(entity, endValue);

            var duration = Parser.ParseInt32(this, 5);
            var bounces = HasArg(LoopedArg) ? -1 : Parser.ParseInt32Argument(this, BouncesArg);
            var bounceDelay = Parser.ParseInt32Argument(this, BounceDelayArg);
            var startDelay = Parser.ParseInt32Argument(this, StartDelayArg);

            tween = new FloatTween() { BounceDelay = bounceDelay, StartDelay = startDelay };
            tween.Start(style, GetStartValue(entity), endValue, duration, bounces);

            SetTween(entity, tween);
        }

        // SetTween
        protected abstract void SetTween(Entity entity, FloatTween tween);

        // IsAwaiting
        public override bool IsAwaiting => tween != null && tween.IsRunning;
    }
}
