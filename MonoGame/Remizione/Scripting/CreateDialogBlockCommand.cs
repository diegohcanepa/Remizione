using Adberration.Scripting;

namespace Remizione.Scripting
{
    // CreateDialogBlockCommand
    // Syntax: [#routine:Routine] [#prevent-quit]
    internal sealed class CreateDialogBlockCommand : NonAwaitableCommand
    {
        private const string preventQuitArg = "#prevent-quit";
        private readonly Script? routine;

        // Constructor
        internal CreateDialogBlockCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, preventQuitArg, RoutineArg)
        {
            routine = Parser.ParseRoutineArgument(this, RoutineArg, null);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var allowQuit = !HasArg(preventQuitArg);
            DialogBlock.Instance = new DialogBlock(routine, allowQuit);
        }
    }
}
