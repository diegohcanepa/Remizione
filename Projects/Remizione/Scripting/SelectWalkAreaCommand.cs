using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // SelectWalkAreaCommand
    // Arguments: {Room} {Name:String}
    internal sealed class SelectWalkAreaCommand : NonAwaitableCommand
    {
        // Constructor
        internal SelectWalkAreaCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            var room = AssertEntityNotNull<GameRoom>(0);
            var walkAreaName = Parser.ParseName(this, 1);

            if (room.WalkAreas.Find(walkAreaName) == null)
            {
                throw new ScriptException(this, "Walk area not found.");
            }
        }

        // OnExecute
        protected override void OnExecute()
        {
            AssertEntity<GameRoom>(0)?.SelectWalkArea(Body.Clauses[1]);
        }
    }
}
