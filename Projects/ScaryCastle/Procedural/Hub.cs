
namespace ScaryCastle
{
    /// <summary>
    /// Hub
    /// </summary>
    public sealed class Hub : GameRoom
    {
        // Constructor
        public Hub(GameSession session, string name)
            : base(session, name)
        {
            UnloadMode = Adberration.UnloadMode.Manual;
        }

        #region Private members

        // LinkGate
        private void LinkGate()
        {
            if (Session.CurrentRun?.CurrentCorridor is RoomNode node)
            {
                var thingName = $"{nameof(RideDoor)}Up*Hub";

                if (Children.Find(thingName) is RideDoor gate)
                    gate.TargetRoom = node.RideRoom;
            }
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            LinkGate();
        }

        #endregion
    }
}
