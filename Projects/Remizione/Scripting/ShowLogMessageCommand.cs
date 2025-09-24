using Adberration.Scripting;

namespace Remizione.Scripting
{
    // ShowLogMessageCommand
    // Arguments: {LogVerb} {MetaItem}
    internal sealed class ShowLogMessageCommand : NonAwaitableCommand
    {
        // Constructor
        public ShowLogMessageCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            Parser.ParseEnum<LogVerb>(this, 0);

            if (MetaItem.Find(Body.Clauses[1]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[1]}' not defined.");
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                var verb = Parser.ParseEnum<LogVerb>(this, 0);
                if (MetaItem.Find(Body.Clauses[1]) is MetaItem metaItem)
                    session.HUD.Log.Show(verb, metaItem.LocalizedDisplayName, metaItem.Image);
            }
        }

        #endregion
    }
}
