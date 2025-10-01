using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AddResistanceCommand
    // Arguments: {DamageType} {float}
    internal sealed class AddResistanceCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddResistanceCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            if (string.IsNullOrWhiteSpace(BeginResistanceTableCommand.ActiveName))
                throw new ScriptException(script, "You need to call begin-damage-resistance-table first.");

            var damageType = Parser.ParseEnum<DamageType>(this, 0);
            var modifier = Parser.ParseFloat(this, 1);

            var table = ResistanceTable.Find(BeginResistanceTableCommand.ActiveName);
            table ??= ResistanceTable.Register(BeginResistanceTableCommand.ActiveName);
            table.SetModifier(damageType, modifier);
        }
    }
}
