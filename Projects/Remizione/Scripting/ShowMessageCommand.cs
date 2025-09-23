using Adberration.Scripting;

namespace Remizione.Scripting
{
    // ShowMessageCommand
    // Arguments: {HUDMessageKind}
    internal sealed class ShowMessageCommand : NonAwaitableCommand
    {
        // Constructor
        public ShowMessageCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1)
        {
            Parser.ParseEnum<HUDMessageKind>(this, 0);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                var value = Parser.ParseEnum<HUDMessageKind>(this, 0);
                session.HUD.Message.Show(value);
            }
        }

        #endregion
    }
}
