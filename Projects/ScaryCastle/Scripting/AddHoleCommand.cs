using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AddHoleCommand
    // Arguments: {Name} to {WalkAreaName} polygon {Polygon} #condition:FlagCondition
    internal sealed class AddHoleCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddHoleCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 5, ConditionArg)
        {
            var room = AssertEntityNotNull<GameRoom>(Script.EntityName);
            var holeName = Parser.ParseName(this, 0);
            AssertKeyword(1, "to");
            var walkAreaName = Parser.ParseName(this, 2);
            AssertKeyword(3, "polygon");
            var vertices = Parser.ParseVector2Array(this, 4);

            if (room.WalkAreas.Find(walkAreaName) is WalkArea walkArea)
                walkArea.AddHole(holeName, Parser.ParseFlagConditionArgument(this, ConditionArg), vertices);
            else
                throw new ScriptException(this, $"The room '{room.Name}' has no walk area.");
        }
    }
}
