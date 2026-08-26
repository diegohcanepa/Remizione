using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;

namespace ScaryCastle
{
    /// <summary>
    /// Door
    /// </summary>
    public class Door : Openable
    {
        private readonly Sprite lockImage = new();

        #region Constructor

        // Constructor
        public Door(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;

            if (name.StartsWith("DoorUp", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = DoorDirection.Up;
            }
            else if (name.StartsWith("DoorDown", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = DoorDirection.Down;
            }
            else if (name.StartsWith("DoorLeft", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = DoorDirection.Left;
            }
            else if (name.StartsWith("DoorRight", StringComparison.OrdinalIgnoreCase))
            {
                DoorDirection = DoorDirection.Right;
            }
            else
            {
                throw new InvalidOperationException("Cannot infere door direction from entity name.");
            }

            CollisionDetection = false;
            DisplayNameKey = "Prop.Door";
            Verb = Verb.Use;
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
        private static string GetVisualAssetName(RoomNode current, RoomNode neighbor, DoorDirection doorDirection)
        {
            var category = neighbor.Category == RoomCategory.Start ? RoomCategory.Standard : neighbor.Category;

            if (current.LockedDoors.TryGetValue(doorDirection, out LockType lockType) && lockType == LockType.GateLever)
                return $"{current.Definition.Theme}_Gate";
            else
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
                    case DoorDirection.Up:
                        Verb = Verb.GoUp;
                        break;

                    // Right
                    case DoorDirection.Right:
                        Verb = Verb.GoRight;
                        break;

                    // Down
                    case DoorDirection.Down:
                        Verb = Verb.GoDown;
                        break;

                    // Left
                    case DoorDirection.Left:
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

        // OnLockTypeChanged
        protected override void OnLockTypeChanged()
        {
            base.OnLockTypeChanged();
            this.lockImage.RenderImage = Atlas?.FindImage($"{DeclaredName}_{LockType}");

            if (LockType == LockType.GateLever)
            {
                Verb = Verb.Examine;
                CloseSound = Sound.Find(SoundNames.DoorGateClose);
                OpenSound = Sound.Find(SoundNames.DoorGateOpen);
            }
            else
            {
                this.Verb = Verb.Use;
            }
        }

        // OnTransform
        protected override void OnTransform(TransformChange change)
        {
            base.OnTransform(change);
            lockImage?.MatchTransform(this.Sprite);
        }

        #endregion

        // CanBeUnlockedWithPocketItem
        [ScriptProperty]
        public bool CanBeUnlockedWithPocketItem => (LockType == LockType.BronzeKey && Session.CurrentRun?.PocketItems.BronzeKeys > 0) ||
                       (LockType == LockType.GoldenKey && Session.GoldenKeys > 0);

        // Connect
        [ScriptMethod(CodingContext.Execution)]
        public void Connect()
        {
            if (TargetRoom != null)
            {
                int roomIndex = Room is ProceduralRoom proceduralRoom ? proceduralRoom.RoomNode.Index : -1;
                var pos = TargetRoom.GetPlayerPosition(roomIndex, out Door? door);

                if (door != null)
                {
                    door.IsOpen = true;
                    door.LockType = LockType.None;
                }

                ConnectCore(TargetRoom, pos);
            }
        }

        // DoorDirection
        [ScriptProperty]
        public DoorDirection DoorDirection { get; }

        // Prepare
        [ScriptMethod]
        public void Prepare()
        {
            if (Room is ProceduralRoom proceduralRoom)
            {
                Sprite.ClearAnimations();

                var assetPrefix = string.Empty;

                if (DoorDirection == DoorDirection.Up && proceduralRoom.RoomNode.Up != null)
                {
                    assetPrefix = GetVisualAssetName(proceduralRoom.RoomNode, proceduralRoom.RoomNode.Up, DoorDirection);
                }
                else if (DoorDirection == DoorDirection.Right && proceduralRoom.RoomNode.Right != null)
                {
                    assetPrefix = GetVisualAssetName(proceduralRoom.RoomNode, proceduralRoom.RoomNode.Right, DoorDirection);
                }
                else if (DoorDirection == DoorDirection.Down && proceduralRoom.RoomNode.Down != null)
                {
                    assetPrefix = GetVisualAssetName(proceduralRoom.RoomNode, proceduralRoom.RoomNode.Down, DoorDirection);
                }
                else if (DoorDirection == DoorDirection.Left && proceduralRoom.RoomNode.Left != null)
                {
                    assetPrefix = GetVisualAssetName(proceduralRoom.RoomNode, proceduralRoom.RoomNode.Left, DoorDirection);
                }

                CloseSound = Sound.Find(SoundNames.DoorGenericClose);
                OpenSound = Sound.Find(SoundNames.DoorGenericOpen);

                var prefix = $"Door_{assetPrefix}_{DoorDirection}_";

                var animation = AddAnimation(AnimationNames.Closed);
                animation.AddFrame(prefix + animation.Name, 1000);

                animation = AddAnimation(AnimationNames.Open);
                animation.AddFrame(prefix + animation.Name, 1000);

                SyncAnimation();
            }
        }

        // TargetRoom
        public ProceduralRoom? TargetRoom { get; set; }

        // TargetRoomPosition
        public Vector2 TargetRoomPosition { get; set; }

        // UnlockWithPocketItem
        [ScriptMethod(CodingContext.Execution)]
        public void UnlockWithPocketItem()
        {
            if (LockType == LockType.BronzeKey && Session.CurrentRun?.PocketItems.BronzeKeys > 0)
            {
                Session.CurrentRun.PocketItems.BronzeKeys--;
                LockType = LockType.None;
            }
            else if (LockType == LockType.GoldenKey && Session.GoldenKeys > 0)
            {
                Session.GoldenKeys--;
                LockType = LockType.None;
            }
        }
    }
}
