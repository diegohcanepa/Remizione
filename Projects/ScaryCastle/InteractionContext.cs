using Engendro.Input;

namespace ScaryCastle
{
    /// <summary>
    /// InteractionContext
    /// </summary>
    public sealed class InteractionContext
    {
        // Constructor
        public InteractionContext(GameSession session)
        {
            this.Session = session;
        }

        #region Private members

        // CanScanTarget
        private bool CanScanTarget()
        {
            // Modal speech bubble active
            if (SpeechBubble.ModalInstance != null)
                return false;

            // Session is awaiting script
            if (Session.IsAwaiting)
                return false;

            // Inventory is active
            if (!Session.IsCurrentScene)
                return false;

            return true;
        }

        // ScanTarget
        private GameThing? ScanTarget()
        {
            if (Session.Room == null)
                return null;

            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);

            for (int i = Session.Room.CulledThings.Count - 1; i >= 0; i--)
            {
                // Exclude player when HeldItem is null
                if (Session.Room.CulledThings[i] == Session.Player && HeldItem == null)
                    continue;

                if (Session.Room.CulledThings[i] is GameThing target && target.CanInteract() && target.RuntimeHotspot.Contains(mousePos))
                    return target;
            }

            return null;
        }

        #endregion

        // HeldItem
        public Item? HeldItem { get; set; }

        // LiftMode
        public bool LiftMode { get; set; }

        // Refresh
        public void Refresh()
        {
            if (Session.Player == null || !Session.Player.IsInCurrentRoom)
            {
                Reset();
                return;
            }

            Target = CanScanTarget() ? ScanTarget() : null;
            if (HeldItem?.Amount == 0)
                HeldItem = null;

            MouseCursorAppearance.Refresh(this);
        }

        // Reset
        public void Reset()
        {
            LiftMode = false;

            if (HeldItem?.Definition.DeselectOnUse == true)
                HeldItem = null;

            Target = null;
            MouseCursorAppearance.Refresh(this);
        }

        // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }
    }
}