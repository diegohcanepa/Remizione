using Adberration.Scripting;
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
        public RideRoom(GameSession session, RoomNode roomNode)
            : base(session, string.Empty, roomNode)
        {
            Zoom = 1.15f;

            AtlasName = roomNode.Definition.Name ?? string.Empty;
            DefaultImageName = AtlasName;
            GlobalLightSize = new(2.2f, 2);
            LightMapColor = new(20, 20, 20);
            LightingSystem = true;

            AddWalkArea("WalkArea", roomNode.Definition.WalkArea);

            // Add placeholders
            foreach (var placeholder in roomNode.Definition.Placeholders)
            {
                AddPlaceholder(placeholder);
            }

            // Add walls
            foreach (var wall in roomNode.Definition.Walls)
            {
                AddWall(wall);
            }
        }

        #endregion

        #region Private members

        // PopulateDoors
        private void PopulateDoors()
        {
            var def = this.RoomNode.Definition;

            // Up
            if (RoomNode.Up != null && def.DoorUp != null && CreateThingClone("RideDoorUp") is RideDoor upDoor)
            {
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = def.DoorUp.Value;
                upDoor.TargetRoom = RoomNode.Up.RideRoom;
            }

            // Left
            if (RoomNode.Left != null && def.DoorLeft != null && CreateThingClone("RideDoorLeft") is RideDoor leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = def.DoorLeft.Value;
                leftDoor.TargetRoom = RoomNode.Left.RideRoom;
            }

            // Right
            if (RoomNode.Right != null && def.DoorRight != null && CreateThingClone("RideDoorRight") is RideDoor rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = def.DoorRight.Value;
                rightDoor.TargetRoom = RoomNode.Right.RideRoom;
            }

            // Down
            if (RoomNode.Down != null)
            {
                if (def.DoorDown != null && CreateThingClone("RideDoorDown") is RideDoor downDoor)
                {
                    doors.Add(downDoor);
                    Children.Add(downDoor);
                    downDoor.Position = def.DoorDown.Value;

                    if (RoomNode.Down != null)
                        downDoor.TargetRoom = RoomNode.Down.RideRoom;
                }
            }
        }

        // PopulateCorridor
        private void PopulateCorridor()
        {
            void AddGate(Prop gate)
            {
                gate.Atlas = Atlas;
                gate.PivotOrigin = RectanglePoint.LeftTop;
                gate.RenderLayer = RenderLayer.Background;
                gate.DepthOffset = -1;
                Children.Add(gate);
            }

            if (Session.FindDeclaredThing("CorridorLeftGate") is Prop leftGate)
            {
                leftGate.DefaultImageName = "LeftGate";
                AddGate(leftGate);
            }

            if (Session.FindDeclaredThing("CorridorRightGate") is Prop rightGate)
            {
                rightGate.DefaultImageName = "RightGate";
                AddGate(rightGate);
            }

            if (Session.FindDeclaredThing("CorridorLeftWall") is Prop leftWall)
            {
                leftWall.Atlas = Atlas;
                Children.Add(leftWall);
            }

            if (Session.FindDeclaredThing("CorridorRightWall") is Prop rightWall)
            {
                rightWall.Atlas = Atlas;
                Children.Add(rightWall);
            }
        }

        #endregion

        #region Protected members

        protected override void OnActivate()
        {
            base.OnActivate();

            if (Session.ScriptLibrary.FindRoutine("CorridorLeftGate-Down") is Script script)
                Session.ScriptProcessor.StartScript(script);
        }

        // OnLoad
        protected override void OnLoad()
        {
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

            if (RoomNode.RoomType == RoomType.Corridor)
                PopulateCorridor();
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
        public static RideRoom CreateInstance(GameSession session, RoomNode roomNode)
        {
            // Get type from AOT registry
            if (Activator.CreateInstance(typeof(RideRoom), session, roomNode) is not RideRoom result)
                throw new InvalidOperationException($"Cannot create instance [{roomNode.Definition.Name}]");

            return result;
        }

        // GetPlayerPosition
        public Vector2 GetPlayerPosition(int previousRoomIndex, out RideDoor? targetDoor)
        {
            targetDoor = null;

            foreach (var door in Children.OfType<RideDoor>())
            {
                if ((previousRoomIndex == -1 && door.DoorDirection == RideDoorDirection.Down) ||
                     door.TargetRoom?.RoomNode.Index == previousRoomIndex)
                {
                    targetDoor = door;
                    return door.GetAnchoredPosition(door.ApproachPosition);
                }
            }

            return Vector2.Zero;
        }
    }
}
