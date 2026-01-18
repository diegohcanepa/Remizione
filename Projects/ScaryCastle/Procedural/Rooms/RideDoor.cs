using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// RideDoor
    /// </summary>
    public class RideDoor : Prop
    {
        private readonly MouseCursorState arrowCursor;
        private readonly ImageSprite lockImage;

        #region Constructor

        // Constructor
        public RideDoor(GameSession session, string name)
            : base(session, name)
        {
            if (name.StartsWith("RideDoorUp", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Up;
                arrowCursor = MouseCursorState.Up;
            }
            else if (name.StartsWith("RideDoorDown", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Down;
                arrowCursor = MouseCursorState.Down;
            }
            else if (name.StartsWith("RideDoorLeft", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Left;
                arrowCursor = MouseCursorState.Left;
            }
            else if (name.StartsWith("RideDoorRight", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Right;
                arrowCursor = MouseCursorState.Right;
            }
            else
            {
                throw new InvalidOperationException("Cannot infere door direction from entity name.");
            }

            Atlas = Atlases.Environment;
            CollisionDetection = false;
            DisplayNameKey = "Prop.Door";
            CloseSound = Sound.Find("DoorClose");
            OpenSound = Sound.Find("DoorOpen");

            this.lockImage = new(Game, Atlas.FindImage($"{DeclaredName}Lock"));

            SetStateHandler(PropState.Closed, Close);
            SetStateHandler(PropState.Locked, Lock);
            SetStateHandler(PropState.Open, Open);
            SetStateHandler(PropState.Unlocked, Unlock);

            InitializeState(PropState.Closed);
        }

        #endregion

        #region Private members

        // Close
        private bool Close()
        {
            if (CloseSound != null)
                PlaySound(CloseSound);
            SyncAnimation();
            Bounce();
            return true;
        }

        // ConnectCore
        private void ConnectCore(GameRoom targetRoom, Vector2 targetPosition)
        {
            if (Session.Player != null)
            {
                Session.Player.Unparent();
                targetRoom.Children.Add(Session.Player);
                Session.Player.Position = targetPosition;
                Session.Camera.FollowTarget(Session.Player, true);
            }

            Session.EnterRoom(targetRoom);
        }

        // Lock
        private bool Lock()
        {
            // Down doors cannot be locked
            return DoorDirection != RideDoorDirection.Down;
        }

        // OnPropStateChanged
        protected override void OnPropStateChanged()
        {
            base.OnPropStateChanged();
            SyncAnimation();
        }

        // Open
        private bool Open()
        {
            if (PropState == PropState.Locked)
                return false;

            if (OpenSound != null)
                PlaySound(OpenSound);

            SyncAnimation();
            Bounce();

            return true;
        }

        // SyncAnimation
        private void SyncAnimation()
        {
            if (PropState == PropState.Open)
                Sprite.Player.Play("Open");
            else
                Sprite.Player.Play("Closed");
        }

        // Unlock
        private bool Unlock()
        {
            Sound.Play(SoundNames.LockOpen);
            return true;
        }

        #endregion

        #region Protected members

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);
            if (PropState == PropState.Locked)
                lockImage.Draw(gameTime);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lockImage?.MatchTransform(this.Sprite);
        }

        #endregion

        // CanInteractWithItem
        public override bool CanInteractWithItem()
        {
            return PropState == PropState.Closed;
        }

        // CloseSound
        [ScriptProperty]
        public Sound? CloseSound { get; set; }

        // Connect
        [ScriptMethod(CodingContext.Execution)]
        public void Connect()
        {
            if (TargetRoom != null)
            {
                int roomIndex = Room is RideRoom rideRoom ? rideRoom.RoomGraph.Index : -1;
                var pos = TargetRoom.GetPlayerPosition(roomIndex, out RideDoor? door);
                door?.PropState = PropState.Open;
                ConnectCore(TargetRoom, pos);
            }
        }

        // DoorDirection
        [ScriptProperty]
        public RideDoorDirection DoorDirection { get; }

        // GetMouseCursorState
        public override MouseCursorState? GetMouseCursorState()
        {
            if (MouseCursor.Target == this && PropState == PropState.Open)
                return arrowCursor;
            else
                return base.GetMouseCursorState();
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }

        // Prepare
        [ScriptMethod]
        public void Prepare()
        {
            Sprite.ClearAnimations();

            if (Room is RideRoom rideRoom)
            {
                var prefix = rideRoom.Definition.Name.Split("_")[0];
                prefix = $"{prefix}Door{DoorDirection}";

                var animation = AddAnimation("Closed");
                animation.AddFrame(prefix + animation.Name, 1000);

                animation = AddAnimation("Open");
                animation.AddFrame(prefix + animation.Name, 1000);
            }
        }

        // TargetRoom
        public RideRoom? TargetRoom { get; set; }

        // TargetRoomPosition
        public Vector2 TargetRoomPosition { get; set; }
    }
}
