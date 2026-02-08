using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // ShowLogMessageCommand
    // Arguments: {LogVerb} {ItemDefinition} [#delay:Integer]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class ShowLogMessageCommand : NonAwaitableCommand
    {
        // Constructor
        public ShowLogMessageCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2, DelayArg)
        {
            Parser.ParseEnum<LogVerb>(this, 0);

            if (ItemDefinition.Definitions.Find(Body.Clauses[1]) == null)
                throw new ScriptException(this, $"Item '{Body.Clauses[1]}' not defined.");

            Parser.ParseInt32Argument(this, DelayArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                var verb = Parser.ParseEnum<LogVerb>(this, 0);
                if (ItemDefinition.Definitions.Find(Body.Clauses[1]) is ItemDefinition definition)
                    session.HUD.Log.Show(verb, definition, Parser.ParseInt32Argument(this, DelayArg));
            }
        }

        #endregion
    }
}
