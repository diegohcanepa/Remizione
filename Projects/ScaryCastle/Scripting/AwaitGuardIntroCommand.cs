using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitGuardIntroCommand
    [ForceAwait]
    internal sealed class AwaitGuardIntroCommand : AwaitableCommand
    {
        private Script? routine;

        // Constructor
        internal AwaitGuardIntroCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Guard == null)
                return;

            routine = session.ScriptLibrary.FindRoutine($"{session.Guard.DeclaredName}-Intro");
            if (routine != null)
                session.AwaitScript(routine);
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            routine = null;
        }

        // IsAwaiting
        public override bool IsAwaiting => routine != null && Session.ScriptProcessor.IsExecutingScript(routine);
    }
}
