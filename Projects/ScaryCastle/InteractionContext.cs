using Engendro;
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
            this.CoinDisplayName = TextRepository.GetValue("Prop.Coin");
        }

        #region Private members

        // CanScanTarget
        private bool CanScanTarget()
        {
            // Modal speech text active
            if (SpeechText.ModalInstance != null)
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
                if (Session.Room.CulledThings[i] == Session.Player)
                {
                    if (HeldItem?.Definition.Behavior == ItemBehavior.PlayerAction)
                        continue;
                }

                if (Session.Room.CulledThings[i] is GameThing target && Session.Room.IsIlluminated(target))
                {
                    if (target.CanInteract() && target.RuntimeHotspot.Contains(mousePos))
                        return target;
                }
            }

            return null;
        }

        #endregion

        // CoinDisplayName
        public string CoinDisplayName { get; }

        // HeldItem
        public Item? HeldItem { get; set; }

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
            if (Target != null)
            {
                if (HeldItem?.Definition.DeselectOnUse == true)
                    HeldItem = null;
            }

            Target = null;
            MouseCursorAppearance.Refresh(this);
        }

        // Session
        public GameSession Session { get; }

        // Target
        public GameThing? Target { get; private set; }
    }
}