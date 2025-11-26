using Engendro;
using Microsoft.Xna.Framework;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public abstract class RideRoom : ProceduralRoom
    {
        private bool lootDropped = false;

        // Constructor
        protected RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            AllowGlobalLight = true;
            AtlasName = $"{graph.RoomStyle}RideRoom";
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

        // OnEnemiesCleared
        protected override void OnEnemiesCleared()
        {
            if (lootDropped)    
                return;

            var dropPosition = WalkArea != null ? WalkArea.Polygon.BoundingRectangleF.Center : BoundingBox.Center;

            if (RoomGraph.HasCoin)
            {
                if (MetaItem.Find(MetaItem.CoinItemName) is MetaItem metaItem)
                    Session.ObjectPools.Pickups.Get()?.Drop(this, dropPosition, metaItem);
            }
            else if (Loot.TryDropLoot(this, dropPosition, GetType().Name, out _))
                lootDropped = true;
        }

        // OnPopulating
        protected override void OnPopulating()
        {
            PopulateDoors();
        }

        #endregion

        // GetPlayerPosition
        public Vector2 GetPlayerPosition(int previousRoomId, out RideDoor? targetDoor)
        {
            targetDoor = null;

            foreach (var thing in Children)
            {
                if (thing is RideDoor door)
                {
                    if (door.TargetRoom == null)
                    {
                        if (previousRoomId == 0)
                            targetDoor = door;
                    }
                    else if (door.TargetRoom.RoomGraph.Id == previousRoomId)
                        targetDoor = door;

                    if (targetDoor != null)
                        return door.GetAbsolutePoint(door.ApproachPosition);
                }
            }

            return Vector2.Zero;
        }

        // HubDoor
        public RideDoor? HubDoor { get; set; }
    }

    /// <summary>
    /// RideRoomWall
    /// </summary>
    public sealed class RideRoomWall : Prop
    {
        // Constructor
        public RideRoomWall(GameSession session, string vertices)
            : base(session, string.Empty)
        {
            Hotspot = new Polygon(vertices);
            HotspotPlacement = PlacementMode.Absolute;
        }
    }
}
