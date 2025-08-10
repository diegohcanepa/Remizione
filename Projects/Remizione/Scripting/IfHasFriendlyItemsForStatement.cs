using Adberration.Scripting;

namespace Remizione.Scripting
{
    // IfHasFriendlyItemsForStatement
    // Arguments: {Target:GameThing}
    internal sealed class IfHasFriendlyItemsForStatement : SelectionStatement
    {
        // Constructor
        internal IfHasFriendlyItemsForStatement(Script script, string source, StatementBody body)
            : base(script, StatementType.If, source, body, 1)
        {
            AssertEntity<GameThing>(0);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            if (Session is GameSession session && Parser.ParseEntity<GameThing>(this, 0) is GameThing thing)
            {
                var items = session.GetFriendlyItems(thing.StaticName);
                return items.Length > 0;
            }
            else
                return false;
        }
    }
}
