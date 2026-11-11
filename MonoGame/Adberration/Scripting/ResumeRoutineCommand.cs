namespace Adberration.Scripting
{
    // ResumeRoutineCommand
    // Arguments: {Routine}
    internal sealed class ResumeRoutineCommand : NonAwaitableCommand
    {
        // Constructor
        internal ResumeRoutineCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertRoutine(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var routine = AssertRoutine(0);
            if (routine != null)
            {
                Session.ScriptProcessor.ResumeScript(routine);
            }
        }
    }
}
