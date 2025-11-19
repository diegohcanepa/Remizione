namespace Remizione
{
    /// <summary>
    /// RoomDescriptor
    /// </summary>
    public class RoomDescriptor
    {
        // Constructor
        public RoomDescriptor(int id, RideRoomKind roomKind)
        {
            Id = id;
            RoomKind = roomKind;
        }

        // Down
        public RoomDescriptor? Down { get; set; }

        // Id
        public int Id { get; }

        // RoomKind
        public RideRoomKind RoomKind { get; }

        // Left
        public RoomDescriptor? Left { get; set; }

        // Right
        public RoomDescriptor? Right { get; set; }

        // Up
        public RoomDescriptor? Up { get; set; }
    }
}
