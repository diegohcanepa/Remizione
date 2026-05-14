using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitLargeHandCommand
    [ForceAwait]
    public sealed class AwaitLargeHandCommand : AwaitableCommand
    {
        private LargeHand? largeHand;

        // Constructor
        internal AwaitLargeHandCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || session.Player == null)
                return;

            largeHand = session.Environment.LargeHand;
            
            largeHand.Hit(session.Player);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => largeHand?.IsBusy == true;
    }
}
