namespace Adberration.Scripting
{
    // PauseRoutineCommand
    // Arguments: {Routine}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class PauseRoutineCommand : NonAwaitableCommand
    {
        // Constructor
        internal PauseRoutineCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertRoutine(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertRoutine(0) is Script script)
            {
                Session.ScriptProcessor.PauseScript(script);
            }
        }
    }
}
