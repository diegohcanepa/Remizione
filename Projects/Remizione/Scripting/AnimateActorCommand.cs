using Adberration.Scripting;
using Engendro;

namespace ScaryCastle.Scripting
{
    // AnimateActorCommand
    // Arguments: {Actor} {AnimationName} [#looped] [#preserve] [#reverse]
    internal sealed class AnimateActorCommand : AwaitableCommand
    {
        private Actor? actor;
        private SpriteAnimation? animation;

        // Constructor
        internal AnimateActorCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, LoopedArg, PreserveArg, ReverseArg)
        {
            AssertEntity<Actor>(0);

            if (HasArg(LoopedArg) && Body.Await)
                throw ScriptExceptionBuilder.EndlessLoop(this);
        }

        // OnExecute
        protected override void OnExecute()
        {
            actor = AssertEntity<Actor>(0);
            if (actor == null)
                return;

            var animationName = Body.Clauses[1];
            var direction = HasArg(ReverseArg) ? AnimationDirection.Reverse : AnimationDirection.Forward;
            animation = actor.Animate(animationName, HasArg(LoopedArg), direction, HasArg(PreserveArg));
        }

        // OnExecutionCompleteds
        protected override void OnExecutionCompleted()
        {
            animation = null;
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return actor != null && animation != null &&
                                           actor.AnimationPlayer.IsPlaying &&
                                           actor.AnimationPlayer.Animation == animation;
        }
    }
}
