namespace Adberration.Scripting
{
    // AwaitAnimationTriggerCommand
    // Arguments: {Entity}
    [ForceAwait]
    internal sealed class AwaitAnimationTriggerCommand : AwaitableCommand
    {
        private Entity? entity;

        // Constructor
        internal AwaitAnimationTriggerCommand(Script script, string source, StatementBody body)
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
            return !(entity?.AnimationPlayer.Frame?.IsTrigger == true);
        }
    }
}
