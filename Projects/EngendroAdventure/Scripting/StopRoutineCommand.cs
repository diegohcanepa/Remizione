namespace EngendroAdventure.Scripting
{
    // StopRoutineCommand
    // Arguments: {Routine}
    internal sealed class StopRoutineCommand : NonAwaitableCommand
    {
        private readonly Script? routine;

        // Constructor
        internal StopRoutineCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            routine = AssertRoutine(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            if (routine != null)
            {
                Session.ScriptProcessor.StopScript(routine);
            }
        }
    }
}
