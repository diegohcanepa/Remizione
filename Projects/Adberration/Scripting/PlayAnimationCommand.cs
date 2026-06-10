using Engendro;

namespace Adberration.Scripting
{
    // PlayAnimationCommand
    // Arguments: {Entity} {AnimationName} [#looped] [#random-frame] [#reverse]
    internal sealed class PlayAnimationCommand : AwaitableCommand
    {
        private SpriteAnimation? animation;
        private Entity? entity;

        // Constructor
        internal PlayAnimationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, RandomFrameArg, ReverseArg, LoopedArg)
        {
            AssertEntity<Entity>(0);

            if (HasArg(LoopedArg) && Body.Await)
                throw ScriptExceptionBuilder.EndlessLoop(this);
        }

        // OnExecute
        protected override void OnExecute()
        {
            entity = AssertEntity<Entity>(0);
            if (entity == null)
                return;

            var animationName = Body.Clauses[1];
            animation = entity.AnimationPlayer.Play(animationName, HasArg(LoopedArg), HasArg(ReverseArg) ? AnimationDirection.Reverse : AnimationDirection.Forward, HasArg(RandomFrameArg));
        }

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return entity != null && animation != null && entity.AnimationPlayer.IsPlaying && entity.AnimationPlayer.Animation == animation;
        }
    }
}
