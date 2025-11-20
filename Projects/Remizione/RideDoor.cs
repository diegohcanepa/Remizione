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
            DisplayNameKey = "Prop.Door";
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

            if (Session.Player != null)
            {
                hubRoom.Children.Add(Session.Player);
                Session.Player.Position = hubDoor.BoundingBox.GetPoint(RectanglePoint.Bottom);
                Session.Camera.FollowTarget(Session.Player, true);
            }

            Session.EnterRoom(hubRoom);
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
            // Go back to Hud
            if (Room is RideRoom rideRoom && rideRoom.HubDoor != null && TargetRoom == null)
            {
                BackToHub(rideRoom.HubDoor);
                return;
            }

            if (TargetRoom == null)
                return;

            if (RunManager.GetRoom(TargetRoom.RoomGraph.Id) is not RideRoom nextRoom)
                return;

            if (Session.Player != null)
            {
                Session.Player.Unparent();
                nextRoom.Children.Add(Session.Player);
                //Session.Player.Position = NextRoomPosition;
                //Session.Player.Direction = NextRoomDirection;
                Session.Camera.FollowTarget(Session.Player, true);
            }

            Session.EnterRoom(nextRoom);

            //OnConnected(NextRoom);
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
