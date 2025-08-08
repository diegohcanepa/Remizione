using Adberration.Scripting;

namespace Remizione.Scripting
{
    // HideOverlayTextCommand
    // Syntax: {Name} [#fade:Integer]
    internal sealed class HideOverlayTextCommand : NonAwaitableCommand
    {
        // Constructor
        internal HideOverlayTextCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 1, FadeArg)
        {
            Parser.ParseName(this, 0);
            Parser.ParseInt32Argument(this, FadeArg);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            var fadeDuration = Parser.ParseInt32Argument(this, FadeArg, 0);
            (Session as GameSession)?.OverlayTexts.Hide(Body.Clauses[0], fadeDuration);
        }

        #endregion
    }
}
