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

        // RequiresPersistence
        protected override bool RequiresPersistence => false;

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            Session.AwaitRoutine("IncomingRideCar-Intro");
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
