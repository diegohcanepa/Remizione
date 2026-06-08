using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitBossIntroCommand
    [ForceAwait]
    internal sealed class AwaitBossIntroCommand : AwaitableCommand
    {
        private Script? routine;

        // Constructor
        internal AwaitBossIntroCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Boss == null)
                return;

            routine = session.ScriptLibrary.FindRoutine($"{session.Boss.DeclaredName}-Intro");
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
        public override bool IsAwaiting() => routine != null && Session.ScriptProcessor.IsExecutingScript(routine);
    }
}
