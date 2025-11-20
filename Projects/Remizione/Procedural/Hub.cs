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
            var prefix = $"{nameof(RideDoor)}Up*";

            // Left door    
            if (Children.Find($"{prefix}Left") is RideDoor leftDoor)
            {
                leftDoor.TargetRoom = RunManager.EntryRooms[0];
                RunManager.EntryRooms[0].HubDoor = leftDoor;
            }

            // Middle door    
            if (Children.Find($"{prefix}Middle") is RideDoor middleDoor)
            {
                middleDoor.TargetRoom = RunManager.EntryRooms[1];
                RunManager.EntryRooms[1].HubDoor = middleDoor;
            }

            // Right door    
            if (Children.Find($"{prefix}Right") is RideDoor rightDoor)
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
            if (!RunManager.HasContent)
            {
                Session.BeginRun();
                base.OnLoad();
                LinkDoors();
            }
            else
                base.OnLoad();
        }

        #endregion
    }
}
