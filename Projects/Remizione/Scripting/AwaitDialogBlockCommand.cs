using Adberration.Scripting;

namespace Remizione.Scripting
{
    // AwaitDialogBlockCommand
    [ForceAwait]
    public sealed class AwaitDialogBlockCommand : AwaitableCommand
    {
        private DialogBlockScene? scene;

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

            scene = new DialogBlockScene(session, DialogBlock.Instance);
            DialogBlock.Instance = null;
            scene.SceneController.Push();
        }

        // OnExecutionCompleted
        protected override void OnExecutionCompleted()
        {
            base.OnExecutionCompleted();
            scene = null;
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting => scene != null && scene.SceneController.IsPushed;
    }
}
