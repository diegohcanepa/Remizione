using Adberration.Scripting;
using Engendro.Audio;

namespace ScaryCastle.Scripting
{
    // LiftOutcomeTargetCommand
    // Syntax: {Source:Actor}
    internal sealed class LiftOutcomeTargetCommand : NonAwaitableCommand
    {
        // Constructor
        internal LiftOutcomeTargetCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<Actor>(0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<Actor>(0) is Actor actor && session.OutcomeTarget is Prop prop)
                actor.ActiveThrowable = prop;
        }

        #endregion
    }
}