using Adberration.Scripting;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// Tower
    /// </summary>
    public class Tower : IsometricProp
    {
        private bool isOpen;
        private const string ClosedState = "Closed";
        private const string OpenState = "Open";

        // Constructor
        public Tower(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
        }

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            isOpen = false;
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
            if (Session.PowerRestored && !isOpen)
                Open();
        }

        #endregion

        // IsOpen
        [ScriptProperty]
        public virtual bool IsOpen => Session.PowerRestored || isOpen;

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (!isOpen && AnimationPlayer.Animation?.Name != OpenState)
            {
                AnimationPlayer.Play(OpenState, false);
                isOpen = true;
                OnOpen();
            }
        }
    }
}
