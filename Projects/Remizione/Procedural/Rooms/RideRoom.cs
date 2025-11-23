using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public abstract class RideRoom : ProceduralRoom
    {
        // Constructor
        protected RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            AllowGlobalLight = true;
            AtlasName = $"RideRoom{graph.RoomStyle}";
            DefaultImageName = AtlasName;
        }

        #region Private members

        // PopulateDoors
        private void PopulateDoors()
        {
            // Up
            if (RoomGraph.Up != null && CreateRuntimeClone("RideDoorUp") is RideDoor upDoor)
            {
                upDoor.Position = DoorUpPosition;
                upDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Up.Id);
                Children.Add(upDoor);
            }

            // Left
            if (RoomGraph.Left != null && CreateRuntimeClone("RideDoorLeft") is RideDoor leftDoor)
            {
                leftDoor.Position = DoorLeftPosition;
                leftDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Left.Id);
                Children.Add(leftDoor);
            }

            // Right
            if (RoomGraph.Right != null && CreateRuntimeClone("RideDoorRight") is RideDoor rightDoor)
            {
                rightDoor.Position = DoorRightPosition;
                rightDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Right.Id);
                Children.Add(rightDoor);
            }

            // Down
            if (RoomGraph.IsRoot || RoomGraph.Down != null)
            {
                if (CreateRuntimeClone("RideDoorDown") is RideDoor downDoor)
                {
                    downDoor.Position = DoorDownPosition;

                    if (RoomGraph.Down != null)
                        downDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Down.Id);

                    Children.Add(downDoor);
                }
            }
        }

        #endregion

        #region Protected members

        // DoorDownPosition
        protected Vector2 DoorDownPosition { get; set; }

        // DoorLeftPosition
        protected Vector2 DoorLeftPosition { get; set; }

        // DoorRightPosition
        protected Vector2 DoorRightPosition { get; set; }

        // DoorUpPosition
        protected Vector2 DoorUpPosition { get; set; }

        // OnPopulating
        protected override void OnPopulating()
        {
            PopulateDoors();
        }

        #endregion

        // GetPlayerPosition
        public Vector2 GetPlayerPosition(int roomId, out RideDoor? targetDoor)
        {
            targetDoor = null;

            foreach (var thing in Children)
            {
                if (thing is RideDoor door && door.TargetRoom != null)
                {
                    if (door.TargetRoom.RoomGraph.Id == roomId)
                    {
                        targetDoor = door;
                        return door.GetAbsolutePoint(door.ApproachPosition);
                    }
                }
            }

            return Vector2.Zero;
        }

        // HubDoor
        public RideDoor? HubDoor { get; set; }
    }
}
