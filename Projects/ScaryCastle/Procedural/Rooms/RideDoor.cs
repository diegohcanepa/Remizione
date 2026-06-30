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
            }
            else if (name.StartsWith("RideDoorDown", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Down;
            }
            else if (name.StartsWith("RideDoorLeft", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Left;
            }
            else if (name.StartsWith("RideDoorRight", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = RideDoorDirection.Right;
            }
            else
            {
                throw new InvalidOperationException("Cannot infere door direction from entity name.");
            }

            CollisionDetection = false;
            DisplayNameKey = "Prop.Door";
            Verb = Verb.Use;

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

        // GetVisualAssetName
        private static string GetVisualAssetName(RoomNode current, RoomNode neighbor)
        {
            var category = neighbor.Category == RoomCategory.Start ? RoomCategory.Standard : neighbor.Category;

            return $"{current.Definition.Theme}_{category}";
        }

        #endregion

        #region Protected members

        // OnClosureStatusChanged
        protected override void OnClosureStatusChanged(bool actionInProgress)
        {
            if (actionInProgress)
                Bounce();

            if (IsOpen)
            {
                switch (DoorDirection)
                {
                    // Up
                    case RideDoorDirection.Up:
                        Verb = Verb.GoUp;
                        break;

                    // Right
                    case RideDoorDirection.Right:
                        Verb = Verb.GoRight;
                        break;

                    // Down
                    case RideDoorDirection.Down:
                        Verb = Verb.GoDown;
                        break;

                    // Left
                    case RideDoorDirection.Left:
                        Verb = Verb.GoLeft;
                        break;

                    default:
                        break;
                }
            }
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

        // IsEmittingLight
        public override bool IsEmittingLight => Room?.HasAmbientLightSources == true ? false : base.IsEmittingLight;

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
                    assetPrefix = GetVisualAssetName(rideRoom.RoomNode, rideRoom.RoomNode.Up);
                }
                else if (DoorDirection == RideDoorDirection.Right && rideRoom.RoomNode.Right != null)
                {
                    assetPrefix = GetVisualAssetName(rideRoom.RoomNode, rideRoom.RoomNode.Right);
                }
                else if (DoorDirection == RideDoorDirection.Down && rideRoom.RoomNode.Down != null)
                {
                    assetPrefix = GetVisualAssetName(rideRoom.RoomNode, rideRoom.RoomNode.Down);

                    //if (!rideRoom.HasAmbientLightSources)
                    {
                        this.AttachedLight = new("Light")
                        {
                            Color = new(240, 181, 65),
                            LightKind = LightKind.Default,
                            PivotOrigin = RectanglePoint.Center,
                            Scale = new(3, 4)
                        };

                        AttachedLightPosition = new(16, 20);
                    }
                }
                else if (DoorDirection == RideDoorDirection.Left && rideRoom.RoomNode.Left != null)
                {
                    assetPrefix = GetVisualAssetName(rideRoom.RoomNode, rideRoom.RoomNode.Left);
                }

                CloseSound = Sound.Find(SoundNames.DoorGenericClose);
                OpenSound = Sound.Find(SoundNames.DoorGenericOpen);

                var prefix = $"RideDoor_{assetPrefix}_{DoorDirection}_";

                var animation = AddAnimation("Closed");
                animation.AddFrame(prefix + animation.Name, 1000);

                animation = AddAnimation("Open");
                animation.AddFrame(prefix + animation.Name, 1000);

                SyncAnimation();
            }
        }

        // TargetRoom
        public RideRoom? TargetRoom { get; set; }

        // TargetRoomPosition
        public Vector2 TargetRoomPosition { get; set; }
    }
}
