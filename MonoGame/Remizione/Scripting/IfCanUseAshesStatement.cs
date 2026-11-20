using Adberration.Scripting;

namespace Remizione.Scripting
{
    // IfCanUseAshesStatement
    internal sealed class IfCanUseAshesStatement : SelectionStatement
    {
        // Constructor
        internal IfCanUseAshesStatement(Script script, string source, StatementBody args)
            : base(script, StatementType.If, source, args, 1)
        {
            AssertEntity<GameThing>(0);
        }

        // Evaluate
        public sealed override bool Evaluate()
        {
            if (Session is not GameSession session)
                return false;

            if (AssertEntity<GameThing>(0) is not { } target)
                return false;

            return target.AshCost <= session.PlayerData.Ashes;
        }
    }
}
