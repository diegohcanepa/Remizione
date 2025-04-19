using EngendroAdventure.Scripting;

namespace Remizione.Scripting
{
    // CreateDialogBlockCommand
    // Syntax: {RoutineName} [#allow-quit]
    internal sealed class CreateDialogBlockCommand : NonAwaitableCommand
    {
        private const string AllowQuitArg = "#allow-quit";

        // Constructor
        internal CreateDialogBlockCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, AllowQuitArg)
        {
            AssertRoutineNotNull(0);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var routine = AssertRoutineNotNull(0);
            var allowQuit = HasArg(AllowQuitArg);

            DialogBlock.Instance = new DialogBlock(routine, allowQuit);
        }
    }
}
