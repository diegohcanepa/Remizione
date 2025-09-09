namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        // Constructor
        public RideRoom(GameSession session, string name)
            : base(session, name)
        {
        }

        // OnUnload
        protected override void OnUnload()
        {
            base.OnUnload();
            Children.Clear();
            Session.CleanUpRuntimeEntities();
        }
    }
}
