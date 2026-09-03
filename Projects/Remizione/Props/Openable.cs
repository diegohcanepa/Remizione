using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
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
        protected virtual void OnClosureStatusChanged(bool actionInProgress)
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
            if (IsOpen)
                Sprite.Player.Play("Open");
            else
                Sprite.Player.Play("Closed");
        }

        #endregion

        // Close
        [ScriptMethod]
        public void Close()
        {
            if (IsOpen)
            {
                if (IsInCurrentRoom)
                {
                    if (CloseSound != null)
                        PlaySound(CloseSound);
                    actionInProgress = true;
                    IsOpen = false;
                    actionInProgress = false;
                }
            }
        }

        // CloseSound
        [ScriptProperty]
        public Sound? CloseSound { get; set; }

        // IsLocked
        [ScriptProperty]
        public bool IsLocked => LockType != LockType.None;

        // IsOpen
        [ScriptProperty]
        public bool IsOpen
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
            if (IsOpen)
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

            if (OpenSound?.PopInstance() is { } soundInstance)
            {
                soundInstance.TransitionAware = false;
                soundInstance.Play();
                Bounce();
            }

            actionInProgress = true;
            IsOpen = true;
            actionInProgress = false;
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }

        // Shake
        public void Shake()
        {
            if (shakeTween.IsRunning)
                return;
            shakeTween.Start(TweenStyle.Linear, Position, Position + (Vector2.One * .5f), 60, 4);
        }

        // Unlock
        [ScriptMethod]
        public void Unlock()
        {
            if (LockType == LockType.None)
                return;

            if (UnlockSound != null)
                PlaySound(UnlockSound);

            actionInProgress = true;
            LockType = LockType.None;
            actionInProgress = false;
        }

        // UnlockSound
        [ScriptProperty]
        public Sound? UnlockSound { get; set; }
    }
}
