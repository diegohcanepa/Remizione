using Adberration.Scripting;

namespace Remizione.Scripting
{
    // BeginLootTableCommand
    // Arguments: {Name}
    internal sealed class BeginLootTableCommand : NonAwaitableCommand
    {
        // Constructor
        internal BeginLootTableCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            if (!string.IsNullOrWhiteSpace(ActiveName))
                throw new ScriptException(script, "Another table is being defined.");

            ActiveName = Parser.ParseName(this, 0);
        }

        // ActiveName
        internal static string ActiveName { get; set; } = string.Empty;
    }
}
