using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitDevilHandCommand
    // Arguments: {Target:GameThing}
    [ForceAwait]
    public sealed class AwaitDevilHandCommand : AwaitableCommand
    {
        private DeityHand? hand;

        // Constructor
        internal AwaitDevilHandCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            AssertEntity<GameThing>(0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session)
                return;

            if (AssertEntity<GameThing>(0) is not { } target)
                return;

            hand = session.Environment.DevilHand;
            hand.Hit(target);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return hand?.IsBusy == true;
        }
    }
}
