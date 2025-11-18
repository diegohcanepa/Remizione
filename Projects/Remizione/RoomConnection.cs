namespace Remizione
{
    /// <summary>
    /// RoomConnection
    /// </summary>
    public abstract class RoomConnection : Prop
    {
        // Constructor
        protected RoomConnection(GameSession session, string name)
            : base(session, name)
        {
        }

        // NextRoom
        public GameRoom? NextRoom { get; set; }

        // PreviousRoom
        public GameRoom? PreviousRoom { get; set; }
    }
}
