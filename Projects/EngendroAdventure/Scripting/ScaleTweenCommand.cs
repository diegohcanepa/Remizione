using Engendro;
using Microsoft.Xna.Framework;

namespace EngendroAdventure.Scripting
{
    // ScaleTweenCommand
    // Arguments: {Entity} {TweenStyle} to {Vector2} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative]
    internal sealed class ScaleTweenCommand : Vector2TweenCommand
    {
        // Constructor
        internal ScaleTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body)
        {
        }

        // GetEndValue
        protected override Vector2 GetEndValue(Entity entity, Vector2 value)
        {
            if (HasArg(RelativeArg))
            {
                return entity.Scale + value;
            }
            else
            {
                return value;
            }
        }

        // GetStartValue
        protected override Vector2 GetStartValue(Entity entity)
        {
            return entity.Scale;
        }

        // SetTween
        protected override void SetTween(Entity entity, Vector2Tween tween)
        {
            entity.Tweens.ScaleTween = tween;
        }
    }
}
