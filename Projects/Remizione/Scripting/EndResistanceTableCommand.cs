using Adberration.Scripting;

namespace Remizione.Scripting
{
    // EndResistanceTableCommand
    // Arguments: {Name}
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
