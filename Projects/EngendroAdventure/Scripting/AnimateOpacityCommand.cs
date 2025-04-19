using Engendro;

namespace EngendroAdventure.Scripting
{
    // AnimateOpacityCommand
    // Arguments: {Entity} to {Opacity:ratio} duration {Integer} [#style:TweenStyle]
    public sealed class AnimateOpacityCommand : AwaitableCommand
    {
        // Constructor
        public AnimateOpacityCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 5, StyleArg)
        {
            AssertEntity<Entity>(0);
            AssertKeyword(1, "to");
            Parser.ParseFloat(this, 2);
            AssertKeyword(3, "duration");
            Parser.ParseInt32(this, 4);
            Parser.ParseEnumArgument<TweenStyle>(this, StyleArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var entity = AssertEntity<Entity>(0);
            if (entity == null)
            {
                return;
            }

            var endValue = Parser.ParseFloat(this, 2);
            var duration = Parser.ParseInt32(this, 4);
            var style = Parser.ParseEnumArgument<TweenStyle>(this, StyleArg);

            entity.Tweens.OpacityTween = FloatTween.Create(style, entity.Opacity, endValue, duration);
        }
    }
}
