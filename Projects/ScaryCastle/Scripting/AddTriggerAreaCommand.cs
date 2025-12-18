using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AddTriggerAreaCommand
    // Arguments: {Name:String} routine {Routine} polygon {Polygon} [#condition:FlagCondition] [#no-await] [#no-stop] [#once] [#on-exit:Routine]
    internal sealed class AddTriggerAreaCommand : NonAwaitableCommand
    {
        private const string NoStopArg = "#no-stop";

        // Constructor
        internal AddTriggerAreaCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 5, ConditionArg, OnExitArg, NoAwaitArg, NoStopArg, OnceArg)
        {
            var room = AssertEntityNotNull<GameRoom>(Script.EntityName);
            Parser.ParseName(this, 0);
            AssertKeyword(1, "routine");
            var routine = AssertRoutineNotNull(2);
            AssertKeyword(3, "polygon");
            var condition = Parser.ParseFlagConditionArgument(this, ConditionArg);
            var vertices = Parser.ParseVector2Array(this, 4);
            var exitRoutine = Parser.ParseRoutineArgument(this, OnExitArg);

            room.AddTriggerArea(body.Clauses[0], routine, exitRoutine, !HasArg(NoAwaitArg), !HasArg(NoStopArg), HasArg(OnceArg), condition, vertices);
        }
    }
}
