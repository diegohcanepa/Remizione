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
            UnloadMode = Adberration.UnloadMode.Manual;
        }

        #region Private members

        // LinkPathDoors
        private void LinkPathDoors()
        {
            var prefix = $"{nameof(RideDoor)}Up*Hub*";

            // Left door    
            if (Children.Find($"{prefix}1") is RideDoor leftDoor)
            {
                leftDoor.TargetRoom = RunManager.EntryRooms[0];
                RunManager.EntryRooms[0].HubDoor = leftDoor;
            }

            // Middle door    
            if (Children.Find($"{prefix}2") is RideDoor middleDoor)
            {
                middleDoor.TargetRoom = RunManager.EntryRooms[1];
                RunManager.EntryRooms[1].HubDoor = middleDoor;
            }

            // Right door    
            if (Children.Find($"{prefix}3") is RideDoor rightDoor)
            {
                rightDoor.TargetRoom = RunManager.EntryRooms[2];
                RunManager.EntryRooms[2].HubDoor = rightDoor;
            }
        }

        #endregion

        #region Protected members

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            LinkPathDoors();
        }

        #endregion
    }
}
