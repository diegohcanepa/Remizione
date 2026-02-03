using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // CombatFeedbackCommand
    // Arguments: {Actor} {"Text"}
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class CombatFeedbackCommand : LocalizableCommand
    {
        // Constructor
        public CombatFeedbackCommand(Script script, string source, StatementBody args)
            : base(script, source, args, 2, LiteralArg, LocalizationIdArg)
        {
            AssertEntity<Actor>(0);
            Parser.ParseQuotedString(this, 1);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (AssertEntity<Actor>(0) is Actor actor)
            {
                string text = GetDisplayText();
                var color = actor.IsPlayer ? ColorPalette.Text.Green : ColorPalette.Text.Highlight;
                actor.Session.HUD.CombatFeedback.Show(text, color, 2500);
            }
        }

        // TextClauseIndex
        protected override int TextClauseIndex => 1;

        #endregion
    }
}
