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
    public class RideDoor : Openable
    {
        private readonly MouseCursorState arrowCursor;
        private readonly Sprite lockImage;

        #region Constructor

        // Constructor
        public RideDoor(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;

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

            CollisionDetection = false;
            DisplayNameKey = "Prop.Door";
            CloseSound = Sound.Find("DoorClose");
            OpenSound = Sound.Find("DoorOpen");

            this.lockImage = new(Atlas?.FindImage($"{DeclaredName}Lock"));
        }

        #endregion

        #region Private members

        // ConnectCore
        private void ConnectCore(GameRoom targetRoom, Vector2 targetPosition)
        {
            if (Session.Player != null)
            {
                Session.Player.Unparent();
                targetRoom.Children.Add(Session.Player);
                Session.Player.Position = targetPosition;
                Session.Camera.Follow(Session.Player, true);
            }

            Session.EnterRoom(targetRoom);
        }

        #endregion

        #region Protected members

        // OnClosureStatusChanged
        protected override void OnClosureStatusChanged(bool actionInProgress)
        {
            if (actionInProgress)
                Bounce();
        }

        // OnDraw
        protected override void OnDraw(GameTime gameTime)
        {
            base.OnDraw(gameTime);

            if (LockType != LockType.None)
                lockImage.Draw(gameTime);
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lockImage?.MatchTransform(this.Sprite);
        }

        #endregion

        // Connect
        [ScriptMethod(CodingContext.Execution)]
        public void Connect()
        {
            if (TargetRoom != null)
            {
                int roomIndex = Room is RideRoom rideRoom ? rideRoom.RoomNode.Index : -1;
                var pos = TargetRoom.GetPlayerPosition(roomIndex, out RideDoor? door);
                door?.IsOpen = true;
                ConnectCore(TargetRoom, pos);
            }
        }

        // DoorDirection
        [ScriptProperty]
        public RideDoorDirection DoorDirection { get; }

        // GetMouseCursorState
        public override MouseCursorState? GetMouseCursorState()
        {
            if (Session.InteractionContext.Target == this && IsOpen)
                return arrowCursor;
            else
                return null;
        }

        // Prepare
        [ScriptMethod]
        public void Prepare()
        {
            if (Room is RideRoom rideRoom)
            {
                Sprite.ClearAnimations();

                var assetPrefix = string.Empty;
                if (DoorDirection == RideDoorDirection.Up && rideRoom.RoomNode.Up != null)
                {
                    assetPrefix = rideRoom.RoomNode.GetDoorAssetName(rideRoom.RoomNode.Up);
                }
                else if (DoorDirection == RideDoorDirection.Right && rideRoom.RoomNode.Right != null)
                {
                    assetPrefix = rideRoom.RoomNode.GetDoorAssetName(rideRoom.RoomNode.Right);
                }
                else if (DoorDirection == RideDoorDirection.Down && rideRoom.RoomNode.Down != null)
                {
                    assetPrefix = rideRoom.RoomNode.GetDoorAssetName(rideRoom.RoomNode.Down);
                }
                else if (DoorDirection == RideDoorDirection.Left && rideRoom.RoomNode.Left != null)
                {
                    assetPrefix = rideRoom.RoomNode.GetDoorAssetName(rideRoom.RoomNode.Left);
                }

                var prefix = $"RideDoor_{assetPrefix}_{DoorDirection}_";

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
