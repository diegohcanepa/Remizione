using Adberration;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ScaryCastle
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public abstract class RideRoom : ProceduralRoom
    {
        private readonly List<RideDoor> doors = [];
        private bool lootDropped;

        // Constructor
        protected RideRoom(GameSession session, RoomGraph graph)
            : base(session, string.Empty, graph)
        {
            AllowGlobalLight = true;
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
            if (RoomGraph.Up != null && DoorAnchorUp != null && CreateRuntimeClone("RideDoorUp") is RideDoor upDoor)
            {
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = DoorAnchorUp.Value;
                upDoor.TargetRoom = RoomGraph.Up.RideRoom;
            }

            // Left
            if (RoomGraph.Left != null && DoorAnchorLeft != null && CreateRuntimeClone("RideDoorLeft") is RideDoor leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = DoorAnchorLeft.Value;
                leftDoor.TargetRoom = RoomGraph.Left.RideRoom;
            }

            // Right
            if (RoomGraph.Right != null && DoorAnchorRight != null && CreateRuntimeClone("RideDoorRight") is RideDoor rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = DoorAnchorRight.Value;
                rightDoor.TargetRoom = RoomGraph.Right.RideRoom;
            }

            // Down
            if (RoomGraph.Down != null || RoomGraph.RoomType == RoomType.Start)
            {
                if (DoorAnchorDown != null && CreateRuntimeClone("RideDoorDown") is RideDoor downDoor)
                {
                    doors.Add(downDoor);
                    Children.Add(downDoor);
                    downDoor.Position = DoorAnchorDown.Value;

                    if (RoomGraph.Down != null)
                        downDoor.TargetRoom = RoomGraph.Down.RideRoom;
                }
            }
        }

        #endregion

        #region Protected members

        // DoorAnchorDown
        protected Vector2? DoorAnchorDown { get; set; }

        // DoorAnchorLeft
        protected Vector2? DoorAnchorLeft { get; set; }

        // DoorAnchorRight
        protected Vector2? DoorAnchorRight { get; set; }

        // DoorAnchorUp
        protected Vector2? DoorAnchorUp { get; set; }

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
            if (DoorAnchorLeft == Vector2.Zero || DoorAnchorDown == Vector2.Zero ||
                DoorAnchorRight == Vector2.Zero || DoorAnchorUp == Vector2.Zero)
            {
                throw new InvalidOperationException($"One or more door anchor points are missing in room [{RoomGraph}].");
            }

            base.OnLoad();

            foreach (var door in doors)
            {
                if (door.DoorDirection is RideDoorDirection.Left or RideDoorDirection.Right or RideDoorDirection.Up)
                {
                    if (door.TargetRoom?.Config.LockType != LockType.None)
                        door.PropState = PropState.Locked;
                }
                else
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

            // Ahora accedemos a .Type del struct registrado
            var entityInfo = AotTypeRegistry.Get(graph.Config.Template);

            // El compilador ya no dará error aquí porque registered.Type está anotado
            var result = Activator.CreateInstance(entityInfo.Type, session, graph) as RideRoom
                ?? throw new InvalidOperationException($"Cannot create instance [{graph.Config.Name}]");

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

        // RegisterTemplates
        public static void RegisterTemplates()
        {
            AotTypeRegistry.Register(typeof(CommonRoom));
        }
    }
}
