namespace Remizione
{
    /// <summary>
    /// Door
    /// </summary>
    public class Door : RoomConnection
    {
        // Constructor
        public Door(GameSession session, string name)
            : base(session, name)
        {
            DisplayNameKey = "Prop.Door";
        }
    }
}
