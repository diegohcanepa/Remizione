namespace EngendroAdventure.Scripting
{
    // AwaitRoutineCommand
    // Arguments: {Name}
    [ForceAwait]
    internal sealed class AwaitRoutineCommand : AwaitableCommand
    {
        private readonly Script routine;

        // Constructor
        internal AwaitRoutineCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            routine = AssertRoutineNotNull(0);
        }

        // IsAwaiting
        public override bool IsAwaiting => Session.ScriptProcessor.IsExecutingScript(routine) && !routine.IsCompleted;
    }
}
