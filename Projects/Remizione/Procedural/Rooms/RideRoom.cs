using Engendro;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public abstract class RideRoom : ProceduralRoom
    {
        private static Dictionary<string, Type> derivedTypes = [];
        private readonly List<RideDoor> doors = [];
        private bool lootDropped;

        // Constructor
        protected RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            AllowGlobalLight = true;
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
                doors.Add(upDoor);
                Children.Add(upDoor);
            }

            // Left
            if (RoomGraph.Left != null && CreateRuntimeClone("RideDoorLeft") is RideDoor leftDoor)
            {
                leftDoor.Position = DoorLeftPosition;
                leftDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Left.Id);
                doors.Add(leftDoor);
                Children.Add(leftDoor);
            }

            // Right
            if (RoomGraph.Right != null && CreateRuntimeClone("RideDoorRight") is RideDoor rightDoor)
            {
                rightDoor.Position = DoorRightPosition;
                rightDoor.TargetRoom = RunManager.GetRoom(RoomGraph.Right.Id);
                doors.Add(rightDoor);
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
                    doors.Add(downDoor);
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
            else
            {
                Loot.TryDropLoot(this, Config.LootTable, dropPosition, out _);
            }

            for (var i = 0; i < doors.Count; i++)
            {
                doors[i].SwitchStateCooldown = (int)RandomHelper.Next(Random, 700, 1500);
            }

            lootDropped = true;
        }

        // OnEnter
        protected override void OnEnter()
        {
            base.OnEnter();
            
            if (EnemyCount > 0)
            {
                for (var i = 0; i < doors.Count; i++)
                {
                    doors[i].SwitchStateCooldown = (int)RandomHelper.Next(Random, 500, 1000);
                }
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            for (var i = 0; i < doors.Count; i++)
            {
                doors[i].IsOpen = true;
            }
        }

        // OnPopulating
        protected override void OnPopulating()
        {
            PopulateDoors();
        }

        #endregion

        // CreateInstance
        public static RideRoom CreateInstance(GameSession session, RoomGraph graph)
        {
            if (graph.Config == null)
                throw new InvalidOperationException($"Missing config in room graph.");

            var type = derivedTypes[graph.Config.Name];
            var result = Activator.CreateInstance(type, session, graph) as RideRoom ?? throw new InvalidOperationException($"Cannot create instance [{graph.Config.Name}]");
            return result;
        }

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
                    {
                        targetDoor = door;
                    }

                    if (targetDoor != null)
                        return door.GetAbsolutePoint(door.ApproachPosition);
                }
            }

            return Vector2.Zero;
        }

        // HubDoor
        public RideDoor? HubDoor { get; set; }

        // RegisterRideRoom
        public static void RegisterRideRoom(Type type)
        {
            derivedTypes.Add(type.Name, type);
        }
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
