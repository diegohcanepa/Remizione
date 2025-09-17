using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Tower
    /// </summary>
    public class Tower : IsometricProp
    {
        private bool isOpened;
        private const string ClosedState = "Closed";
        private const string OpenState = "Open";

        // Constructor
        public Tower(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
            this.PlacementPhase = PlacementPhase.Connections;
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            isOpened = false;
            AnimationPlayer.Play(ClosedState);
        }

        // OnOpen
        protected virtual void OnOpen()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            if (Session.PowerRestored && !isOpened)
                Open();
        }

        #endregion

        // IsOpen
        [ScriptProperty]
        public virtual bool IsOpen => Session.PowerRestored || isOpened;

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (!isOpened && AnimationPlayer.Animation?.Name != OpenState)
            {
                AnimationPlayer.Play(OpenState, false);
                OnOpen();
            }
        }
    }
}
