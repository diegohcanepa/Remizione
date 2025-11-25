using Adberration.Scripting;

namespace Remizione.Scripting
{
    // BeginResistanceTableCommand
    // Arguments: {Name} [#modifier:Integer]
    internal sealed class BeginResistanceTableCommand : NonAwaitableCommand
    {
        // Constructor
        internal BeginResistanceTableCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, ModifierArg)
        {
            if (!string.IsNullOrWhiteSpace(ActiveName))
                throw new ScriptException(script, "Another table is being defined.");

            ActiveName = Parser.ParseName(this, 0);

            var defaultModifier = Parser.ParseInt32Argument(this, ModifierArg, 1);
            ResistanceTable.Register(ActiveName, defaultModifier);
        }

        // ActiveName
        internal static string ActiveName { get; set; } = string.Empty;
    }
}
