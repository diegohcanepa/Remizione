using Adberration.Scripting;

namespace Remizione.Scripting
{
    /// <summary>
    /// AwaitBonfireSceneCommand
    /// </summary>
    [ForceAwait]
    public sealed class AwaitBonfireSceneCommand : AwaitableCommand
    {
        // Constructor
        internal AwaitBonfireSceneCommand(Script script, string source, StatementBody body)
            : base(script, source, body, 0)
        {
        }

        #region Protected members

        // OnExecute
        protected override void OnExecute()
        {
            if (Session is GameSession session)
                session.Overlays.ShowBonfire();
        }

        #endregion

        // IsAwaiting
        public override bool IsAwaiting()
        {
            return Game.SceneManager.CurrentScene is BonfireScene;
        }
    }
}
