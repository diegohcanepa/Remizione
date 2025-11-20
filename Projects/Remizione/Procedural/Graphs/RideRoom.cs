using Engendro;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        private static readonly Dictionary<RideRoomKind, string> walkAreasByType = [];

        // Static constructor
        static RideRoom()
        {
            walkAreasByType[RideRoomKind.Default] = "207,46;237,111;5,111;36,46;83,46;88,44;164,44;170,46";
        }

        // Constructor
        public RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            AllowGlobalLight = true;
            AtlasName = $"RideRoom{graph.RoomKind}";
            DefaultImageName = AtlasName;
        }

        #region Private members

        // GetDoorPosition
        private Vector2 GetDoorPosition(RideDoorDirection direction)
        {
            if (RoomGraph.RoomKind == RideRoomKind.Default)
            {
                if (direction == RideDoorDirection.Up)
                    return new Vector2(120, 41);

                else if (direction == RideDoorDirection.Left)
                    return new Vector2(21, 80);

                else if (direction == RideDoorDirection.Right)
                    return new Vector2(220, 80);

                else if (direction == RideDoorDirection.Down)
                    return new Vector2(152, 114);
            }

            return Vector2.Zero;
        }

        // PopulateDoors
        private void PopulateDoors()
        {
            // Up
            if (RoomGraph.Up != null && CreateRuntimeClone("RideDoorUp") is RideDoor upDoor)
            {
                upDoor.Position = GetDoorPosition(RideDoorDirection.Up);
                upDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Up.Id);
                Children.Add(upDoor);
            }

            // Left
            if (RoomGraph.Left != null && CreateRuntimeClone("RideDoorLeft") is RideDoor leftDoor)
            {
                leftDoor.Position = GetDoorPosition(RideDoorDirection.Left);
                leftDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Left.Id);
                Children.Add(leftDoor);
            }

            // Right
            if (RoomGraph.Right != null && CreateRuntimeClone("RideDoorRight") is RideDoor rightDoor)
            {
                rightDoor.Position = GetDoorPosition(RideDoorDirection.Right);
                rightDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Right.Id);
                Children.Add(rightDoor);
            }

            // Down
            if (RoomGraph.IsRoot || RoomGraph.Down != null)
            {
                if (CreateRuntimeClone("RideDoorDown") is RideDoor downDoor)
                {
                    downDoor.Position = GetDoorPosition(RideDoorDirection.Down);

                    if (RoomGraph.Down != null)
                        downDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Down.Id);

                    Children.Add(downDoor);
                }
            }
        }

        #endregion

        #region Protected members

        // OnPopulating
        protected override void OnPopulating()
        {
            PopulateDoors();
        }

        // OnSetupWalkArea
        protected override void OnSetupWalkArea()
        {
            if (walkAreasByType.TryGetValue(RoomGraph.RoomKind, out string? vertices))
                AddWalkArea("Default", ReadOnlyPolygon.GetVertices(vertices));
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
