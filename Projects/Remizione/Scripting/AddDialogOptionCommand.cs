using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AddDialogOptionCommand
    // Syntax: {Id:Integer} {"Text"} [#condition:FlagCondition] [#lid:Integer] [#required-options:Id[,Id...]
    internal sealed class AddDialogOptionCommand : LocalizableCommand
    {
        private const string RequiredOptionsArg = "#required-options";

        // Constructor
        internal AddDialogOptionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ConditionArg, LocalizationIdArg, RequiredOptionsArg)
        {
            Parser.ParseInt32(this, 0);
            Parser.ParseQuotedString(this, 1);
            Parser.ParseInt32ArrayArgument(this, RequiredOptionsArg);
            Parser.ParseFlagConditionArgument(this, ConditionArg);
        }

        // GetTextEmitterName()
        protected override string GetTextEmitterName() => "Player";

        // OnExecute
        protected override void OnExecute()
        {
            var block = DialogBlock.Instance ?? throw new ScriptException(this, "No dialog block has been created.");
            var id = Parser.ParseInt32(this, 0);
            var text = GetDisplayText();
            var condition = Parser.ParseFlagConditionArgument(this, ConditionArg);
            int[] requiredOptions = Parser.ParseInt32ArrayArgument(this, RequiredOptionsArg);

            block.Insert(block.Count, id, text, condition, requiredOptions);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 1;
    }
}
