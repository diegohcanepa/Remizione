using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// RideDoor
    /// </summary>
    public class RideDoor : Prop
    {
        private bool isOpen;
        private readonly Vector2Tween scaleTween = new();
        private readonly FloatTween xTween = new();

        // Constructor
        public RideDoor(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
            CollisionDetection = false;
            DisplayNameKey = "Verb.Enter";
            CloseSound = Sound.Find("DoorClose");
            OpenSound = Sound.Find("DoorOpen");
            SyncAnimation();
        }

        #region Private members

        // BackToHub
        private void BackToHub(RideDoor hubDoor)
        {
            if (Session.GetEntity<Hub>("Hub") is not Hub hubRoom)
                return;

            ConnectCore(hubRoom, hubDoor.BoundingBox.GetPoint(RectanglePoint.Bottom));
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

        // SyncAnimation
        private void SyncAnimation()
        {
            if (isOpen)
                Sprite.Player.Play("Open");
            else
                Sprite.Player.Play("Closed");
        }

        #endregion

        #region Protected members

        // OnUpdate
        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            AllowInteraction = Room?.EnemyCount == 0;

            if (AllowInteraction)
            {
                if (!IsOpen && Used)
                    Open();
            }
            else
            {
                if (IsOpen)
                    Close();
            }
        }

        #endregion

        // Close
        [ScriptMethod]
        public void Close()
        {
            if (CloseSound != null)
                PlaySound(CloseSound);

            SyncAnimation();

            scaleTween.Start(TweenStyle.QuadraticInOut, Scale, new Vector2(1f, .96f), 100, 2);
            xTween.Start(TweenStyle.QuadraticInOut, X, X - 1, 50, 4);

            Tweens.ScaleTween = scaleTween;
            Tweens.XTween = xTween;

            IsOpen = false;
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
                Vector2 pos = Vector2.Zero;
                int roomId = 0;
                if (Room is RideRoom rideRoom)
                    roomId = rideRoom.RoomGraph.Id;

                pos = TargetRoom.GetPlayerPosition(roomId, out RideDoor? door);
                if (door != null)
                    door.IsOpen = true;

                ConnectCore(TargetRoom, pos);
            }
            else if (Room is RideRoom rideRoom && rideRoom.HubDoor != null)
            {
                BackToHub(rideRoom.HubDoor);
            }
        }

        // IsOpen
        [ScriptProperty]
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if (value != isOpen)
                {
                    isOpen = value;
                    if (isOpen)
                        Used = true;
                    SyncAnimation();
                }
            }
        }

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (OpenSound != null)
                PlaySound(OpenSound);

            SyncAnimation();

            IsOpen = true;
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }

        // TargetRoom
        public RideRoom? TargetRoom { get; set; }

        // TargetRoomPosition
        public Vector2 TargetRoomPosition { get; set; }

        // Used
        [ScriptProperty]
        public bool Used { get; set; }
    }
}
