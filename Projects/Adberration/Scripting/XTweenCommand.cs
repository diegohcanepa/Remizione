using Engendro;

namespace Adberration.Scripting
{
    // XTweenCommand
    // Arguments: {Entity} {TweenStyle} to {float} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative]
    [ScriptStatement(CodingContext.Execution)]
    public sealed class XTweenCommand : FloatTweenCommand
    {
        // Constructor
        internal XTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body)
        {
        }

        // GetEndValue
        protected override float GetEndValue(Entity entity, float value)
        {
            return HasArg(RelativeArg) ? entity.X + value : value;
        }

        // GetStartValue
        protected override float GetStartValue(Entity entity)
        {
            return entity.X;
        }

        // SetTween
        protected override void SetTween(Entity entity, FloatTween tween)
        {
            entity.Tweens.XTween = tween;
        }
    }
}
