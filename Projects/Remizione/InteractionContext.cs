using Engendro.Input;

namespace Remizione
{
    /// <summary>
    /// InteractionContext
    /// </summary>
    public sealed class InteractionContext
    {
        private readonly MouseCursorAppearance mouseCursorAppearance;

        #region Constructor

        // Constructor
        public InteractionContext(GameSession session)
        {
            this.Session = session;
            this.mouseCursorAppearance = new(this);
        }

        #endregion

        #region Private members

        // ScanTarget
        private GameThing? ScanTarget()
        {
            if (Session.Room == null)
                return null;

            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);

            for (int i = Session.Room.CulledThings.Count - 1; i >= 0; i--)
            {
                if (Session.Room.CulledThings[i] == Session.Player)
                {
                    if (HeldItem == null)
                        continue;
                }

                if (Session.Room.CulledThings[i] is GameThing target)
                {
                    if (!target.IsMoving && target.CanInteract() && target.RuntimeHotspot.Contains(mousePos))
                        return target;
                }
            }

            return null;
        }

        #endregion

        // HeldItem
        public Item? HeldItem { get; set; }

        // Refresh
        public void Refresh()
        {
            Target = Session.IsGameplayActive ? ScanTarget() : null;

            if (HeldItem?.Amount == 0)
                HeldItem = null;

            mouseCursorAppearance.Refresh();
        }

        // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }
    }
}