namespace ScaryCastle
{
    /// <summary>
    /// CastkeTrapdoor
    /// </summary>
    public sealed class CastleTrapdoor : Openable
    {
        #region Constructor

        // Constructor
        public CastleTrapdoor(GameSession session, string name)
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
