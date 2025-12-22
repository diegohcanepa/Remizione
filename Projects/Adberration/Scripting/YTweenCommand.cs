using Engendro;

namespace Adberration.Scripting
{
    // YTweenCommand
    // Arguments: {Entity} {TweenStyle} to {float} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative]
    public sealed class YTweenCommand : FloatTweenCommand
    {
        // Constructor
        internal YTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body)
        {
        }

        // GetEndValue
        protected override float GetEndValue(Entity entity, float value)
        {
            return HasArg(RelativeArg) ? entity.Y + value : value;
        }

        // GetStartValue
        protected override float GetStartValue(Entity entity)
        {
            return entity.Y;
        }

        // SetTween
        protected override void SetTween(Entity entity, FloatTween tween)
        {
            entity.Tweens.YTween = tween;
        }
    }
}
