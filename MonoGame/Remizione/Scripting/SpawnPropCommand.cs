using Adberration.Scripting;

namespace Remizione.Scripting
{
    // SpawnPropCommand
    // Syntax: {ClassName}
    internal sealed class SpawnPropCommand : NonAwaitableCommand
    {
        // Constructor
        internal SpawnPropCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            var name = Parser.ParseName(this, 0);

            // Check if declaration script exists
            var declarationScript = Session.ScriptLibrary.FindDeclaration(name) ?? throw new ScriptException(this, $"The entity class '{name}' is not declared.");

            // Check if entity is cloneable
            if (!declarationScript.Cloneable)
                throw new ScriptException($"The entity '{name}' cannot create clones. Use the '{ScriptSyntax.CloneableKeyword}' keyword.");

        }

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Player?.Room == null)
                return;

            var name = Parser.ParseName(this, 0);
            var prop = session.CreateThingClone<Prop>(name);
            prop.Position = session.Player.Position;
            session.Player.Room.Children.Add(prop);
        }
    }
}
