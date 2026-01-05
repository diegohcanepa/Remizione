namespace Adberration.Scripting
{
    // AwaitOpacityTweenCommand
    // Arguments: {Entity}
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitOpacityTweenCommand : AwaitableCommand
    {
        private Entity? entity;

        // Constructor
        internal AwaitOpacityTweenCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseEntity<Entity>(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            entity = Parser.ParseEntity<Entity>(this, 0);
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            entity = null;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => entity != null && entity.Tweens.IsTweeningOpacity;
    }
}
