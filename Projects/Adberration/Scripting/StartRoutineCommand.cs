namespace Adberration.Scripting
{
    // StartRoutineCommand
    // Arguments: {Routine} [#scope:LifetimeScope]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class StartRoutineCommand : AwaitableCommand
    {
        private Script? routine;

        // Constructor
        internal StartRoutineCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, ScopeArg)
        {
            AssertRoutine(0);
            Parser.ParseEnumArgument<LifetimeScope>(this, ScopeArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            routine = AssertRoutine(0);

            if (routine == null || Session.ScriptProcessor.IsExecutingScript(routine))
                return;

            var scope = Parser.ParseEnumArgument(this, ScopeArg, LifetimeScope.Room);

            if (scope == LifetimeScope.Room)
            {
                if (!Body.Await)
                    Session.Room?.RegisterRoutine(routine);
            }

            if (Body.Await && Session.IsAwaitingScript(Script))
            {
                Session.AwaitScript(routine);
            }
            else
            {
                Session.ScriptProcessor.StartScript(routine);
                routine = null;
            }
        }

        // IsAwaiting
        public override bool IsAwaiting => routine != null && !routine.IsCompleted;
    }
}
