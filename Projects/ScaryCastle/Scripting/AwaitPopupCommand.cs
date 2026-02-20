using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitPopupCommand
    // Syntax: {"Title"} {"Text"}
    [ForceAwait]
    [ScriptStatement(CodingContext.Execution)]
    internal sealed class AwaitPopupCommand : AwaitableCommand
    {
        private PopupScene? scene;

        // Constructor
        internal AwaitPopupCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 2)
        {
            Parser.ParseQuotedString(this, 0);
            Parser.ParseQuotedString(this, 1);
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
            {
                PopupScene popup = new(session, Parser.ParseQuotedString(this, 0), Parser.ParseQuotedString(this, 1), InputBindings.Close);
                Game.SceneManager.Push(popup);
            }
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            scene = null;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => scene != null && scene.IsCurrentScene;
    }
}
