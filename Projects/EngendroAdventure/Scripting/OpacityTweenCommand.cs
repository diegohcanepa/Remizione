using Engendro;
using Microsoft.Xna.Framework;

namespace EngendroAdventure.Scripting
{
    // OpacityTweenCommand
    // Arguments: {Entity} {TweenStyle} to {float} duration {Integer} [#bounces:Integer] [#bounce-delay:Integer] [#looped] [#relative] [#start-delay:Integer] [#unparent]
    internal sealed class OpacityTweenCommand : FloatTweenCommand
    {
        // Constructor
        internal OpacityTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body, UnparentArg)
        {
        }

        // GetEndValue
        protected override float GetEndValue(Entity entity, float value)
        {
            if (HasArg(RelativeArg))
            {
                return MathHelper.Clamp(entity.Opacity + value, 0, 1);
            }
            else
            {
                return value;
            }
        }

        // GetStartValue
        protected override float GetStartValue(Entity entity)
        {
            return entity.Opacity;
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            if (HasArg(UnparentArg) && AssertEntity<Entity>(0) is Entity entity)
            {
                entity.Unparent();
            }
        }

        // SetTween
        protected override void SetTween(Entity entity, FloatTween tween)
        {
            entity.Tweens.OpacityTween = tween;
        }
    }
}
