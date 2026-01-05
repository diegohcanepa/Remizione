namespace Adberration.Scripting
{
    // AwaitOutcomeCommand
    // Arguments: {Thing}
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitOutcomeCommand : AwaitableCommand
    {
        private Script? script;

        // Constructor
        internal AwaitOutcomeCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Thing>(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var thing = AssertEntity<Thing>(0);
            script = thing?.PerformOutcome();
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            script = null;
        }

        // IsAwaiting
        public override bool IsAwaiting
        {
            get
            {
                if (script == null)
                {
                    return false;
                }
                else
                {
                    return Session.ScriptProcessor.IsExecutingScript(script);
                }
            }
        }
    }
}
