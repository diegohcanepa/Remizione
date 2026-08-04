using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ShowLogMessageCommand
    // Arguments: {LogVerb} {ItemDefinition} [#warning]
    internal sealed class ShowLogMessageCommand : NonAwaitableCommand
    {
        // Constructor
        public ShowLogMessageCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, WarningArg)
        {
            Parser.ParseEnum<LogVerb>(this, 0);

            if (ItemDefinition.Container.Find(Body.Clauses[1]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[1]}' not defined.");

            Parser.ParseInt32Argument(this, DelayArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            // TODO: Check
            /*
            if (Session is GameSession session)
            {
                var verb = Parser.ParseEnum<LogVerb>(this, 0);
                if (ItemDefinition.Container.Find(Body.Clauses[1]) is ItemDefinition definition)
                    session.TextHUD.Log.Show(verb, definition, HasArg(WarningArg));
            }
            */
        }

        #endregion
    }
}
