using Engendro;
using Engendro.Audio;
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
        private bool lootDropped;

        // Constructor
        public RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            if (graph.Config is not RoomConfig config)
                throw new InvalidOperationException();

            AllowGlobalLight = true;
            Zoom = 1.1f;

            AtlasName = graph.Config?.Name ?? string.Empty;
            DefaultImageName = AtlasName;

            AddWalkArea("WalkArea", config.WalkArea);

            DoorDown = config.DoorDown;
            DoorLeft = config.DoorLeft;
            DoorRight = config.DoorRight;
            DoorUp = config.DoorUp;

            // Add placeholders
            foreach (var placeholder in config.Placeholders)
            {
                AddPlaceholder(placeholder);
            }

            // Add walls
            foreach (var wall in config.Walls)
            {
                AddWall(wall);
            }
        }

        #region Private members

        // DropCoin
        private void DropCoin()
        {
            if (RoomGraph.HasCoin && MetaItem.Find(MetaItem.CoinItemName) is MetaItem coin)
            {
                Session.Inventory.Add(coin);
                Session.HUD.SackSlot.AnimateItem(coin, GetDropLootPosition());
                Session.HUD.Log.Show(LogVerb.Found, coin);
                coin.PickupSound?.Play();
                RoomGraph.HasCoin = false;
            }
        }

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
            if (RoomGraph.Down != null || RoomGraph.RoomType == RoomType.Start)
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
            MetaItem? drop;
            if (RoomGraph.RoomType == RoomType.Coin)
            {
                drop = MetaItem.Find(MetaItem.CoinItemName);
            }
            else
            {
                // 1. Roll de probabilidad: ¿Esta sala da premio?
                // 20% de base es un buen número para empezar.
                Ratio dropChance = .2f;

                // Sumamos la suerte del jugador si tiene un gadget/pasivo
                if (Session.Inventory.Gadget is Item gadget)
                    dropChance += gadget.MetaItem.Effect.LuckBonus;

                // Si el roll falla (el número es mayor a la chance), salimos sin spawnear nada
                if (!dropChance.Roll())
                    return;

                drop = Loot.Get(Session, Config, null, ItemCategory.Pickup);
                RoomGraph.HeartCount++;
            }

            if (drop != null)
                Session.ObjectPools.Pickups.Get()?.Drop(this, GetDropLootPosition(), drop);
        }

        // OnEnemiesCleared
        protected override void OnEnemiesCleared()
        {
            if (lootDropped)
                return;

            DropLoot();

            DropCoin();

            lootDropped = true;

            // Open all doors
            for (var i = 0; i < doors.Count; i++)
            {
                doors[i].SwitchStateCooldown = (int)RandomHelper.Next(Random, 700, 1500);
            }
        }

        // OnEntering
        protected override void OnEntering()
        {
            if (RoomGraph.RoomType == RoomType.Start)
                AudioManager.Music.PlayTag("Run", 3000);

            RoomGraph.Visited = true;

            Session.HUD.MiniMap.CurrentRoom = RoomGraph;

            if (EnemyCount > 0)
            {
                for (var i = 0; i < doors.Count; i++)
                {
                    if (EnemyCount > 0)
                        doors[i].SwitchStateCooldown = (int)RandomHelper.Next(Random, 500, 900);
                }
            }

            if (EnemyCount == 0)
                DropCoin();
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

            foreach (var door in doors)
            {
                door.PropState = PropState.Open;

                if (door.DoorDirection is RideDoorDirection.Left or RideDoorDirection.Right or RideDoorDirection.Up)
                {
                    if (door.TargetRoom?.Config.LockType != LockType.None)
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
            if (graph.Config == null)
                throw new InvalidOperationException($"Missing config in room graph.");

            // Get type from AOT registry
            var result = Activator.CreateInstance(typeof(RideRoom), session, graph) as RideRoom;
            if (result == null)
                throw new InvalidOperationException($"Cannot create instance [{graph.Config.Name}]");

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
