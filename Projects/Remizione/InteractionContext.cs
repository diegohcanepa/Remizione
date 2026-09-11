using Engendro.Input;

namespace Remizione
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

        // ScanTarget
        private GameThing? ScanTarget()
        {
            if (Session.Room == null)
                return null;

            var mousePos = InputManager.DefaultPlayer.Mouse.WorldPosition(Session.Camera);

            for (int i = Session.Room.CulledThings.Count - 1; i >= 0; i--)
            {
                // Exclude player when HeldItem is null
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
            if (Session.Player is not { IsInCurrentRoom: true })
            {
                Reset();
                MouseCursorAppearance.Refresh(this);
                return;
            }

            Target = Session.IsGameplayActive ? ScanTarget() : null;

            if (HeldItem?.Amount == 0)
                HeldItem = null;

            MouseCursorAppearance.Refresh(this);
        }

        // Reset
        public void Reset()
        {
            if (Target is { IsGoToVerb: false } && HeldItem?.Definition.DeselectOnUse == true)
                HeldItem = null;

            Target = null;
        }

        // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }
    }
}