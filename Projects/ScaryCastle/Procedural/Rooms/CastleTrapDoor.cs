namespace ScaryCastle
{
    /// <summary>
    /// CastkeTrapDoor
    /// </summary>
    public sealed class CastleTrapDoor : Openable
    {
        #region Constructor

        // Constructor
        public CastleTrapDoor(GameSession session, string name)
            : base(session, name)
        {
            Atlas = Atlases.Props;
            CollisionDetection = false;
            DisplayNameKey = "Prop.TrapDoor";
            Verb = Verb.Use;
        }

        #endregion
    }
}
