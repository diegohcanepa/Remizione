using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace ScaryCastle
{
    /// <summary>
    /// Openable
    /// </summary>
    public class Openable : Prop
    {
        private bool actionInProgress;
        private readonly Vector2Tween shakeTween = new();

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

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            if (shakeTween.IsRunning)
            {
                var pos = Position;
                Position = shakeTween.CurrentValue;
                base.OnDraw(gameTime);
                Position = pos;
            }
            else
            {
                base.OnDraw(gameTime);
            }
        }

        // OnLockTypeChanged
        protected virtual void OnLockTypeChanged()
        {
        }

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            shakeTween.Update(gameTime);
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
        [ScriptProperty]
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
                if (IsInCurrentRoom)
                {
                    Shake();
                    if (LockedSound != null)
                        PlaySound(LockedSound);
                }
                return;
            }

            if (OpenSound != null)
            {
                Bounce();
                PlaySound(OpenSound);
            }

            actionInProgress = true;
            ClosureState = ClosureState.Open;
            actionInProgress = false;

            return;
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }

        // Shake
        public void Shake()
        {
            if (shakeTween.IsRunning)
                return;
            shakeTween.Start(TweenStyle.Linear, Position, Position + Vector2.One * .5f, 60, 4);
        }

        // Unlock
        public void Unlock()
        {
        }
    }
}
