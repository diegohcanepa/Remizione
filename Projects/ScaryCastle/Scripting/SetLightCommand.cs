using Adberration.Scripting;
using Engendro;

namespace ScaryCastle.Scripting
{
    // SetLightCommand
    // Arguments: {Room} {Name:String} {SwitchState} [#immediate]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class SetLightCommand : NonAwaitableCommand
    {
        // Constructor
        internal SetLightCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3, ImmediateArg)
        {
            var room = AssertEntityNotNull<GameRoom>(0);
            var name = Parser.ParseName(this, 1);

            if (room.Lights.Find(name) == null)
                throw new ScriptException(this, $"Light '{name}' does not exist.");

            Parser.ParseEnum<SwitchState>(this, 2);
        }

        // OnExecute
        protected override void OnExecute()
        {
            var room = AssertEntityNotNull<GameRoom>(0);
            if (room?.Lights.Find(Body.Clauses[1]) is Light light)
            {
                if (Parser.ParseEnum<SwitchState>(this, 2) == SwitchState.On)
                    light.TurnOn();
                else
                    light.TurnOff(HasArg(ImmediateArg));
            }
        }
    }
}
