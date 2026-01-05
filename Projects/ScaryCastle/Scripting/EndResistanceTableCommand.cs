using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // EndResistanceTableCommand
    // Arguments: {Name}
    [ScriptStatement(CodingContext.Initialization)]
    internal sealed class EndResistanceTableCommand : NonAwaitableCommand
    {
        // Constructor
        internal EndResistanceTableCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
            BeginResistanceTableCommand.ActiveName = string.Empty;
        }
    }
}
