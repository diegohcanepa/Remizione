using Adberration.Scripting;
using Engendro.Audio;

namespace ScaryCastle
{
    /// <summary>
    /// Openable
    /// </summary>
    public class Openable : Prop
    {
        private bool actionInProgress;

        // Constructor
        public Openable(GameSession session, string name)
            : base(session, name)
        {
        }

        #region Protected members

        // OnClosureStatusChanged
        protected virtual void OnClosureStatusChanged(bool isAction)
        {
        }

        // OnLockTypeChanged
        protected virtual void OnLockTypeChanged()
        {
        }

        // SyncAnimation
        protected virtual void SyncAnimation()
        {
            if (ClosureState == ClosureState.Open)
                Sprite.Player.Play("Open");
            else
                Sprite.Player.Play("Closed");
        }

        #endregion

        // Close
        [ScriptMethod]
        public void Close()
        {
            if (ClosureState == ClosureState.Open)
            {
                if (CloseSound != null && IsInCurrentRoom)
                {
                    PlaySound(CloseSound);
                    actionInProgress = true;
                    ClosureState = ClosureState.Closed;
                    actionInProgress = false;
                }
            }
        }

        // CloseSound
        [ScriptProperty]
        public Sound? CloseSound { get; set; }

        // ClosureState
        [ScriptProperty]
        public ClosureState ClosureState
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    OnClosureStatusChanged(actionInProgress);
                    SyncAnimation();
                }
            }
        }

        // IsClosed
        [ScriptProperty]
        public bool IsClosed => ClosureState == ClosureState.Closed;

        // IsLocked
        [ScriptProperty]
        public bool IsLocked => ClosureState == ClosureState.Locked;

        // IsOpen
        [ScriptProperty]
        public bool IsOpen => ClosureState == ClosureState.Open;

        // LockedSound
        [ScriptProperty]
        public Sound? LockedSound { get; set; }

        // LockType
        public LockType LockType
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value;
                    OnLockTypeChanged();
                }
            }
        }

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (ClosureState == ClosureState.Open)
                return;

            if (LockType != LockType.None)
            {
                if (LockedSound != null && IsInCurrentRoom)
                    PlaySound(LockedSound);
                return;
            }

            if (OpenSound != null)
                PlaySound(OpenSound);

            actionInProgress = true;
            ClosureState = ClosureState.Open;
            actionInProgress = false;

            return;
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }
    }
}
