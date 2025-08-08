using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EndLootTableCommand
    // Arguments: {Name}
    internal sealed class EndLootTableCommand : NonAwaitableCommand
    {
        // Constructor
        internal EndLootTableCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
            BeginLootTableCommand.ActiveName = string.Empty;
        }
    }
}
