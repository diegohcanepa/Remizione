using Engendro;

namespace EngendroAdventure.Scripting
{
    // RotationTweenCommand
    // Arguments: {Entity} {TweenStyle} to {float} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#radians] [#relative]
    internal sealed class RotationTweenCommand : FloatTweenCommand
    {
        // Constructor
        internal RotationTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body, RadiansArg)
        {
        }

        // GetEndValue
        protected override float GetEndValue(Entity entity, float value)
        {
            if (HasArg(RelativeArg))
            {
                return entity.X + value;
            }
            else
            {
                return value;
            }
        }

        // GetStartValue
        protected override float GetStartValue(Entity entity)
        {
            return HasArg(RadiansArg) ? entity.Rotation : entity.Degrees;
        }

        // SetTween
        protected override void SetTween(Entity entity, FloatTween tween)
        {
            entity.Tweens.RotationTween = tween;
        }
    }
}
