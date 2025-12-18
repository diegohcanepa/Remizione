using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AddWalkAreaCommand
    // Arguments: {Name:String} polygon {Polygon}
    internal sealed class AddWalkAreaCommand : NonAwaitableCommand
    {
        // Constructor
        internal AddWalkAreaCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 3)
        {
            var room = AssertEntityNotNull<GameRoom>(Script.EntityName);

            Parser.ParseName(this, 0);
            AssertKeyword(1, "polygon");
            room.AddWalkArea(body.Clauses[0], Parser.ParseVector2Array(this, 2));
        }
    }
}
