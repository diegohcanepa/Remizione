namespace Remizione
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
        }

        #region Private members

        // LinkDoors
        private void LinkDoors()
        {
            var prefix = $"{nameof(RideDoor)}*";

            // Left door    
            if (Children.Find($"{prefix}Left") is RideDoor leftDoor)
                leftDoor.NextRoom = RunManager.EntryRooms[0];

            // Middle door    
            if (Children.Find($"{prefix}Middle") is RideDoor middleDoor)
                middleDoor.NextRoom = RunManager.EntryRooms[1];

            // Right door    
            if (Children.Find($"{prefix}Right") is RideDoor rightDoor)
                rightDoor.NextRoom = RunManager.EntryRooms[2];
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            Session.BeginRun();
            base.OnLoad();
            LinkDoors();
        }

        #endregion
    }
}
