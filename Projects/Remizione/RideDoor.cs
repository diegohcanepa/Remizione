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

        // Constructor
        public RideDoor(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Environment;
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
                Sprite.Player.Play("Close");
        }

        #endregion

        // Close
        [ScriptMethod]
        public void Close()
        {
            if (CloseSound != null)
                PlaySound(CloseSound);

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
                if (Room is RideRoom rideRoom)
                {
                    pos = TargetRoom.GetPlayerPosition(rideRoom.RoomGraph.Id, out RideDoor? door);
                    if (door != null)
                        door.IsOpen = true;
                }

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
                    SyncAnimation();
                }
            }
        }

        // TargetRoom
        public RideRoom? TargetRoom { get; set; }

        // TargetRoomPosition
        public Vector2 TargetRoomPosition { get; set; }

        // Open
        [ScriptMethod]
        public void Open()
        {
            if (OpenSound != null)
                PlaySound(OpenSound);

            IsOpen = true;
        }

        // OpenSound
        [ScriptProperty]
        public Sound? OpenSound { get; set; }
    }
}
