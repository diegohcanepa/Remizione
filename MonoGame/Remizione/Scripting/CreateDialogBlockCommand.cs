using Adberration.Scripting;
using Engendro;

namespace Remizione.Scripting
{
    // CreateDialogBlockCommand
    // Syntax: [#routine:Routine] [#alignment:HorizontalAlignment] [#prevent-quit]
    internal sealed class CreateDialogBlockCommand : NonAwaitableCommand
    {
        private const string noQuitArg = "#no-quit";
        private readonly Script? routine;

        // Constructor
        internal CreateDialogBlockCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0, AlignmentArg, noQuitArg, RoutineArg)
        {
            routine = Parser.ParseRoutineArgument(this, RoutineArg, null);
            Parser.ParseEnumArgument<HorizontalAlignment>(this, AlignmentArg);
            Parser.ParseQuotedStringArgument(this, TitleArg);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var allowQuit = !HasArg(noQuitArg);
            var alignment = Parser.ParseEnumArgument(this, AlignmentArg, HorizontalAlignment.Center);

            DialogBlock.Instance = new DialogBlock(routine, allowQuit, alignment);
        }
    }
}
