using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        private readonly List<RideDoor> doors = [];

        #region Constructor

        // Constructor
        public RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            Zoom = 1.15f;

            AtlasName = graph.Definition.Name ?? string.Empty;
            DefaultImageName = AtlasName;
            GlobalLightSize = new(2.2f, 2);
            LightMapColor = new(50, 50, 50);
            LightingSystem = true;

            AddWalkArea("WalkArea", graph.Definition.WalkArea);

            DoorDown = graph.Definition.DoorDown;
            DoorLeft = graph.Definition.DoorLeft;
            DoorRight = graph.Definition.DoorRight;
            DoorUp = graph.Definition.DoorUp;

            // Add placeholders
            foreach (var placeholder in graph.Definition.Placeholders)
            {
                AddPlaceholder(placeholder);
            }

            // Add walls
            foreach (var wall in graph.Definition.Walls)
            {
                AddWall(wall);
            }
        }

        #endregion

        #region Private members

        // PopulateDoors
        private void PopulateDoors()
        {
            // Up
            if (RoomGraph.Up != null && DoorUp != null && CreateThingClone("RideDoorUp") is RideDoor upDoor)
            {
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = DoorUp.Value;
                upDoor.TargetRoom = RoomGraph.Up.RideRoom;
            }

            // Left
            if (RoomGraph.Left != null && DoorLeft != null && CreateThingClone("RideDoorLeft") is RideDoor leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = DoorLeft.Value;
                leftDoor.TargetRoom = RoomGraph.Left.RideRoom;
            }

            // Right
            if (RoomGraph.Right != null && DoorRight != null && CreateThingClone("RideDoorRight") is RideDoor rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = DoorRight.Value;
                rightDoor.TargetRoom = RoomGraph.Right.RideRoom;
            }

            // Down
            if (RoomGraph.Down != null)
            {
                if (DoorDown != null && CreateThingClone("RideDoorDown") is RideDoor downDoor)
                {
                    doors.Add(downDoor);
                    Children.Add(downDoor);
                    downDoor.Position = DoorDown.Value;

                    if (RoomGraph.Down != null)
                        downDoor.TargetRoom = RoomGraph.Down.RideRoom;
                }
            }
        }

        #endregion

        #region Protected members

        // DoorDown
        private Vector2? DoorDown { get; set; }

        // DoorLeft
        private Vector2? DoorLeft { get; set; }

        // DoorRight
        private Vector2? DoorRight { get; set; }

        // DoorUp
        private Vector2? DoorUp { get; set; }

        // OnEnter
        protected override void OnEnter()
        {
            base.OnEnter();

            // TODO: Check
            //if (RoomGraph.RoomType == RoomType.Start)
            //    AudioManager.Music.PlayTag("Run", 3000);

            RoomGraph.Visited = true;
            Session.HUD.MiniMap.CurrentRoom = RoomGraph;
        }

        // OnLoad
        protected override void OnLoad()
        {
            // Check door anchors
            if (DoorLeft == Vector2.Zero || DoorDown == Vector2.Zero ||
                DoorRight == Vector2.Zero || DoorUp == Vector2.Zero)
            {
                throw new InvalidOperationException($"One or more door anchor points are missing in room [{RoomGraph}].");
            }

            base.OnLoad();

            var index = 0;
            while (true)
            {
                if (Atlas?.FindImage($"View{index + 1}") == null)
                    break;
                else
                    index++;
            }

            if (index > 0)
            {
                var animation = AddAnimation("View");
                animation.AddFrame($"View{Random.Shared.Next(1, index + 1)}", 10000);
            }

            foreach (var door in doors)
            {
                if (door.DoorDirection is RideDoorDirection.Left or RideDoorDirection.Right or RideDoorDirection.Up)
                {
                    // TODO: Check
                    //if (door.TargetRoom?.Definition.LockType != LockType.None)
                    //    door.LockType = door.TargetRoom.Definition.LockType;
                }
            }
        }

        // OnPopulating
        protected override void OnPopulating()
        {
            PopulateDoors();
        }

        // OnPopulated
        protected override void OnPopulated()
        {
            base.OnPopulated();

            // Prepare doors
            var doors = new List<RideDoor>(Children.OfType<RideDoor>());
            for (var i = 0; i < doors.Count; i++)
            {
                doors[i].Prepare();
            }

            // Remove things that collides with doors
            var removeList = new List<GameThing>();
            foreach (var child in Children.OfType<GameThing>())
            {
                if (child is RideDoor)
                    continue;

                for (var i = 0; i < doors.Count; i++)
                {
                    if (child.BoundingBox.Intersects(doors[i].BoundingBox))
                        removeList.Add(child);
                }
            }

            for (var i = 0; i < removeList.Count; i++)
            {
                removeList[i].Unparent();
            }
        }

        #endregion

        // CreateInstance
        public static RideRoom CreateInstance(GameSession session, RoomGraph graph)
        {
            // Get type from AOT registry
            if (Activator.CreateInstance(typeof(RideRoom), session, graph) is not RideRoom result)
                throw new InvalidOperationException($"Cannot create instance [{graph.Definition.Name}]");

            return result;
        }

        // GetPlayerPosition
        public Vector2 GetPlayerPosition(int previousRoomIndex, out RideDoor? targetDoor)
        {
            targetDoor = null;

            foreach (var door in Children.OfType<RideDoor>())
            {
                if ((previousRoomIndex == -1 && door.DoorDirection == RideDoorDirection.Down) ||
                     door.TargetRoom?.RoomGraph.Index == previousRoomIndex)
                {
                    targetDoor = door;
                    return door.GetAnchoredPosition(door.ApproachPosition);
                }
            }

            return Vector2.Zero;
        }

        // HubDoor
        public RideDoor? HubDoor { get; set; }
    }
}
