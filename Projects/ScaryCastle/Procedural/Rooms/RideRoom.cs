using Engendro;
using Microsoft.Xna.Framework;
using ScaryCastle.Procedural;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public abstract class RideRoom : ProceduralRoom
    {
        private static readonly Dictionary<string, (Type, string[])> derivedTypes = [];
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
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = DoorAnchorUp;
                upDoor.TargetRoom = RoomGraph.Up.RideRoom;
            }

            // Left
            if (RoomGraph.Left != null && CreateRuntimeClone("RideDoorLeft") is RideDoor leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = DoorAnchorLeft;
                leftDoor.TargetRoom = RoomGraph.Left.RideRoom;
            }

            // Right
            if (RoomGraph.Right != null && CreateRuntimeClone("RideDoorRight") is RideDoor rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = DoorAnchorRight;
                rightDoor.TargetRoom = RoomGraph.Right.RideRoom;
            }

            // Down
            if (RoomGraph.Down != null && CreateRuntimeClone("RideDoorDown") is RideDoor downDoor)
            {
                doors.Add(downDoor);
                Children.Add(downDoor);
                downDoor.Position = DoorAnchorDown;
                downDoor.TargetRoom = RoomGraph.Down.RideRoom;
            }
        }

        // Register
        private static void Register(Type type, string[] placeholderNames)
        {
            derivedTypes.Add(type.Name, (type, placeholderNames));
        }

        #endregion

        #region Protected members

        // DoorAnchorDown
        protected Vector2 DoorAnchorDown { get; set; }

        // DoorAnchorLeft
        protected Vector2 DoorAnchorLeft { get; set; }

        // DoorAnchorRight
        protected Vector2 DoorAnchorRight { get; set; }

        // DoorAnchorUp
        protected Vector2 DoorAnchorUp { get; set; }

        // OnEnemiesCleared
        protected override void OnEnemiesCleared()
        {
            if (lootDropped)
                return;

            var dropPosition = WalkArea != null ? WalkArea.Polygon.BoundingRectangleF.Center : BoundingBox.Center;

            if (RoomGraph.RoomType == RoomType.Coin)
            {
                if (MetaItem.Find(MetaItem.CoinItemName) is MetaItem metaItem)
                    Session.ObjectPools.Pickups.Get()?.Drop(this, dropPosition, metaItem);
            }
            else
            {
                if (Loot.TryDropLoot(this, Config.LootTable, dropPosition, out _))
                    RoomGraph.SackCount++;
            }

            for (var i = 0; i < doors.Count; i++)
            {
                doors[i].SwitchStateCooldown = (int)RandomHelper.Next(Random, 700, 1500);
            }

            lootDropped = true;
        }

        // OnEntering
        protected override void OnEntering()
        {
            RoomGraph.Visited = true;

            Session.HUD.MiniMap.CurrentRoom = RoomGraph;

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
            // Check door anchors
            if (DoorAnchorLeft == Vector2.Zero || DoorAnchorDown == Vector2.Zero ||
                DoorAnchorRight == Vector2.Zero || DoorAnchorUp == Vector2.Zero)
                throw new InvalidOperationException($"One or more door acnhor points are missing in room [{RoomGraph}].");

            base.OnLoad();

            foreach (var door in doors)
            {
                if (door.DoorDirection is RideDoorDirection.Left or RideDoorDirection.Right)
                {
                    if (door.TargetRoom?.Config.LockType != LockType.None)
                        door.PropState = PropState.Locked;
                }

                door.PropState = PropState.Open;
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
            if (graph.Config == null)
                throw new InvalidOperationException($"Missing config in room graph.");

            var type = derivedTypes[graph.Config.Template].Item1;
            var result = Activator.CreateInstance(type, session, graph) as RideRoom ?? throw new InvalidOperationException($"Cannot create instance [{graph.Config.Name}]");
            return result;
        }

        // GetPlayerPosition
        public Vector2 GetPlayerPosition(int previousRoomIndex, out RideDoor? targetDoor)
        {
            targetDoor = null;

            foreach (var door in Children.OfType<RideDoor>())
            {
                if (door.TargetRoom == null)
                {
                    if (previousRoomIndex == 0)
                        targetDoor = door;
                }
                else if (door.TargetRoom.RoomGraph.Index == previousRoomIndex)
                {
                    targetDoor = door;
                }

                if (targetDoor != null)
                    return door.GetAbsolutePoint(door.ApproachPosition);
            }

            return Vector2.Zero;
        }

        // HasPlaceholder
        public static bool HasPlaceholder(string typeName, string placeholderName)
        {
            if (derivedTypes.TryGetValue(typeName, out var result))
            {
                for (var i = 0; i < result.Item2.Length; i++)
                {
                    if (string.Compare(placeholderName, result.Item2[i], StringComparison.InvariantCulture) == 0)
                        return true;
                }
            }

            return false;
        }

        // HubDoor
        public RideDoor? HubDoor { get; set; }

        // IsRegistered
        public static bool IsRegistered(string typeName)
        {
            return derivedTypes.ContainsKey(typeName);
        }

        // RegisterTemplates
        public static void RegisterTemplates()
        {
            Register(typeof(CommonRoom), CommonRoom.GetPlaceholderNames());
        }
    }
}
