using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitExamineItemCommand
    [ForceAwait]
    public sealed class AwaitExamineItemCommand : AwaitableCommand
    {
        private readonly GameSession? gameSession;

        // Constructor
        internal AwaitExamineItemCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
            gameSession = Session as GameSession;
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (gameSession?.HoveredItem != null)
                gameSession.Player?.Say(gameSession.HoveredItem.Description, true);
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return gameSession?.Player != null && gameSession.Player.HasSpeechText;
        }
    }
}
