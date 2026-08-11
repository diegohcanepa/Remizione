using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AddDialogOptionCommand
    // Syntax: {Id:Integer} {"Text"} [#condition:FlagCondition] [#lid:Integer] [#requires-read:Id[,Id...]] [#requires-read:Id[,Id...]]
    internal sealed class AddDialogOptionCommand : LocalizableCommand
    {
        private const string RequiresReadArg = "#requires-read";

        // Constructor
        internal AddDialogOptionCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, ConditionArg, LocalizationIdArg, RequiresReadArg)
        {
            Parser.ParseInt32(this, 0);
            Parser.ParseQuotedString(this, 1);
            Parser.ParseInt32ArrayArgument(this, RequiresReadArg);
            Parser.ParseFlagConditionArgument(this, ConditionArg);
        }

        // GetTextEmitterName()
        protected override string GetTextEmitterName()
        {
            return "Player";
        }

        // OnExecute
        protected override void OnExecute()
        {
            var block = DialogBlock.Instance ?? throw new ScriptException(this, "No dialog block has been created.");
            var id = Parser.ParseInt32(this, 0);
            var text = GetDisplayText();
            var condition = Parser.ParseFlagConditionArgument(this, ConditionArg);
            int[] requiredReadOptions = Parser.ParseInt32ArrayArgument(this, RequiresReadArg);

            block.Insert(block.Count, id, text, condition, requiredReadOptions);
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 1;
    }
}
