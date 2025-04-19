using Engendro;
using Microsoft.Xna.Framework;

namespace EngendroAdventure.Scripting
{
    // PositionTweenCommand
    // Arguments: {Entity} {TweenStyle} to {Vector2} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative]
    internal sealed class PositionTweenCommand : Vector2TweenCommand
    {
        // Constructor
        internal PositionTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body)
        {
        }

        // GetEndValue
        protected override Vector2 GetEndValue(Entity entity, Vector2 value)
        {
            if (HasArg(RelativeArg))
            {
                return entity.Position + value;
            }
            else
            {
                return value;
            }
        }

        // GetStartValue
        protected override Vector2 GetStartValue(Entity entity)
        {
            return entity.Position;
        }

        // SetTween
        protected override void SetTween(Entity entity, Vector2Tween tween)
        {
            entity.Tweens.PositionTween = tween;
        }
    }
}
