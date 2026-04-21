using System;

namespace ScaryCastle
{
    /// <summary>
    /// SideRoom
    /// </summary>
    public sealed class SideRoom : RideRoom
    {
        // Constructor
        public SideRoom(GameSession session, RoomNode roomNode)
            : base(session, roomNode)
        {
            if (roomNode.RoomType != RoomType.SideRoom)
                throw new InvalidOperationException($"Invalid room type for SideRoom: {roomNode.RoomType}");
        }

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();
            if (Session.PreviousRoom is CorridorRoom)
                Session.HUD.Countdown.Start(GameSettings.CorridorRoomCooldown);
        }
    }
}
