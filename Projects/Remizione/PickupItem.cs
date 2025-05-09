namespace Remizione
{
    /// <summary>
    /// PickupItem
    /// </summary>
    public class PickupItem : GameThing
    {
        // Constructor
        public PickupItem(GameSession session, string name)
            : base(session, name)
        {
            this.Atlas = Atlases.Environment;
        }
    }
}
