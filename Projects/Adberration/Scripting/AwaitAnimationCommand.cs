namespace Adberration.Scripting
{
    // AwaitAnimationCommand
    // Arguments: {Entity}
    [ForceAwait]
    internal sealed class AwaitAnimationCommand : AwaitableCommand
    {
        private Entity? entity;

        // Constructor
        internal AwaitAnimationCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Entity>(0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            entity = AssertEntity<Entity>(0);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return entity != null && entity.AnimationPlayer.IsPlaying;
        }
    }
}
