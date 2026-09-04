using Adberration.Scripting;

namespace Remizione.Scripting
{
    // ShowMessageCommand
    // Arguments: {MessageKind}
    internal sealed class ShowMessageCommand : NonAwaitableCommand
    {
        // Constructor
        public ShowMessageCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseEnum<MessageKind>(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                var value = Parser.ParseEnum<MessageKind>(this, 0);
                session.HUD?.Message.Show(value);
            }
        }

        #endregion
    }
}
