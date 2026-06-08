using Adberration.Scripting;

namespace ScaryCastle.Scripting
{
    // AwaitDialogBlockCommand
    [ForceAwait]
    public sealed class AwaitDialogBlockCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitDialogBlockCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is not GameSession session || DialogBlock.Instance == null || DialogBlock.Instance.AvailableOptions.Count == 0)
                return;

            session.ShowDialogMenu(DialogBlock.Instance);
            DialogBlock.Instance = null;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting() => Game.SceneManager.CurrentScene is DialogBlockScene;
    }
}
