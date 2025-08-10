using Engendro;
using Adberration.Scripting;

namespace Remizione.Scripting
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

        // IsAwaiting
        public override bool IsAwaiting => actor != null &&
                                           animation != null &&
                                           actor.AnimationPlayer.IsPlaying &&
                                           actor.AnimationPlayer.Animation == animation;
    }
}
