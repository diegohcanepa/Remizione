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

        // Constructor
        public RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            if (graph.Definition is not RoomDefinition definition)
                throw new InvalidOperationException();

            Zoom = 1.1f;

            AtlasName = graph.Definition?.Name ?? string.Empty;
            DefaultImageName = AtlasName;
            GlobalLightSize = new(2.2f, 2);
            LightMapColor = new(50, 50, 50);
            LightingSystem = true;

            AddWalkArea("WalkArea", definition.WalkArea);

            DoorDown = definition.DoorDown;
            DoorLeft = definition.DoorLeft;
            DoorRight = definition.DoorRight;
            DoorUp = definition.DoorUp;

            // Add placeholders
            foreach (var placeholder in definition.Placeholders)
            {
                AddPlaceholder(placeholder);
            }
        }

        #region Private members

        // PopulateDoors
        private void PopulateDoors()
        {
            // Up
            if (RoomGraph.Up != null && DoorUp != null && CreateRuntimeClone("RideDoorUp") is RideDoor upDoor)
            {
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = DoorUp.Value;
                upDoor.TargetRoom = RoomGraph.Up.RideRoom;
            }

            // Left
            if (RoomGraph.Left != null && DoorLeft != null && CreateRuntimeClone("RideDoorLeft") is RideDoor leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = DoorLeft.Value;
                leftDoor.TargetRoom = RoomGraph.Left.RideRoom;
            }

            // Right
            if (RoomGraph.Right != null && DoorRight != null && CreateRuntimeClone("RideDoorRight") is RideDoor rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = DoorRight.Value;
                rightDoor.TargetRoom = RoomGraph.Right.RideRoom;
            }

            // Down
            if (RoomGraph.Down != null)
            {
                if (DoorDown != null && CreateRuntimeClone("RideDoorDown") is RideDoor downDoor)
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

        // DropLoot
        protected override void DropLoot()
        {
            // 1. Roll de probabilidad: ¿Esta sala da premio?
            // 20% de base es un buen número para empezar.
            Ratio dropChance = .2f;

            // Sumamos la suerte del jugador si tiene un gadget/pasivo
            // TODO: Reimplement
            /*
            if (Session.Inventory.PassiveItem is Item gadget)
                dropChance += gadget.MetaItem.Effect.LuckBonus;
            */

            // Si el roll falla (el número es mayor a la chance), salimos sin spawnear nada
            if (!dropChance.Roll())
                return;

            /*
            var drop = Loot.Get(Session, Config, null, ItemCategory.Pickup);
            RoomGraph.HeartCount++;

            if (drop != null)
                Session.ObjectPools.Pickups.Get()?.Drop(this, GetDropLootPosition(), drop);
            */
        }

        // OnEntering
        protected override void OnEntering()
        {
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
                    if (door.TargetRoom?.Definition.LockType != LockType.None)
                        door.PropState = PropState.Locked;
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
            if (graph.Definition == null)
                throw new InvalidOperationException($"Missing definition in room graph.");

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
                    return door.GetAbsolutePoint(door.ApproachPosition);
                }
            }

            return Vector2.Zero;
        }

        // HubDoor
        public RideDoor? HubDoor { get; set; }
    }
}
