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
    public class RideRoom : ProceduralRoom
    {
        private readonly List<RideDoor> doors = [];
        private readonly Prop foreground;

        #region Constructor

        // Constructor
        public RideRoom(GameSession session, RoomNode roomNode)
            : base(session, string.Empty, roomNode)
        {
            Zoom = 1.15f;
            AtlasName = roomNode.Definition.Name ?? string.Empty;
            DefaultImageName = AtlasName;
            DustParticleKind = DustParticleKind.Ash;
            LightMapColor = roomNode.Definition.LightMapColor;
            LightingSystem = true;

            AddWalkArea("WalkArea", roomNode.Definition.WalkArea);

            // Add lights
            var index = 0;
            foreach (var lightDescriptor in roomNode.Definition.Lights)
            {
                var light = AddLight($"Light{index}__");
                light.Ambient = true;
                light.Color = lightDescriptor.Color;
                light.Position = lightDescriptor.Position;
                light.Scale = lightDescriptor.Scale;
                index++;
            }

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

            // Foreground
            this.foreground = new(Session, string.Empty)
            {
                PivotOrigin = RectanglePoint.LeftTop,
                RenderLayer = RenderLayer.Foreground
            };
        }

        #endregion

        #region Private members

        // DistributeBronzeKeys
        private void DistributeBronzeKeys()
        {
            if (RoomNode.BronzeKeys <= 0)
                return;

            // Collect actors
            var actors = new List<Actor>();
            foreach (var actor in Children.OfType<Actor>())
            {
                if (actor.IsDead || actor.IsPlayer || actor.Faction != Faction.Evil)
                    continue;

                if (actor.Definition?.DropTrigger == LootDropTrigger.OnImpact)
                    continue;

                actors.Add(actor);
            }
            actors.Shuffle();

            // Collect pottery
            var potteryList = new List<Pottery>();
            foreach (var pottery in Children.OfType<Pottery>())
            {
                if (pottery.CanHideLoot)
                    potteryList.Add(pottery);
            }
            potteryList.Shuffle();

            var pendingKeys = RoomNode.BronzeKeys;

            // Randomly hide a bronze key under a pot (Chance = 20%)
            if (potteryList.Count > 0 && DiceExpression.Dice10.Roll() <= 2)
            {
                if (potteryList.GetRandomItem() is Pottery pot)
                {
                    DropBronzeKey(pot.Position - new Vector2(0, 2));
                    pendingKeys--;
                    if (pendingKeys <= 0)
                        return;
                }
            }

            // Randomly drop a bronze key in the room (Chance = 20%)
            if (DiceExpression.Dice10.Roll() <= 2)
            {
                DropBronzeKey();
                pendingKeys--;
                if (pendingKeys <= 0)
                    return;
            }

            // Distribute bronze keys among actors
            while (pendingKeys > 0 && actors.Count > 0)
            {
                if (actors.Count > 0)
                {
                    actors[0].ItemReward = ItemDefinition.Definitions.Get(ItemNames.BronzeKey);
                    actors.RemoveAt(0);
                    pendingKeys--;
                    if (pendingKeys <= 0)
                        return;
                }
            }

            // If there are still pending keys, drop them in the room
            while (pendingKeys > 0)
            {
                DropBronzeKey();
                pendingKeys--;
            }
        }

        // DropBronzeKey
        private void DropBronzeKey(Vector2? position = null)
        {
            var key = CreateThingClone<Prop>(ItemNames.BronzeKey);
            Children.Add(key);

            if (position.HasValue)
            {
                key.Position = position.Value;
            }
            else if (WalkArea != null)
            {
                key.Position = WalkArea.RandomWalkablePoint(Random, 30);
            }
        }

        // LockDoorsAccordingly
        private void LockDoorsAccordingly()
        {
            foreach (var door in doors)
            {
                if (RoomNode.LockedDoors.TryGetValue(door.DoorDirection, out LockType lockType))
                    door.LockType = lockType;
            }
        }

        // PopulateDoors
        private void PopulateDoors()
        {
            var def = this.RoomNode.Definition;

            // Up
            if (RoomNode.Up != null && def.DoorUp != null && CreateThingClone<RideDoor>("RideDoorUp") is RideDoor upDoor)
            {
                doors.Add(upDoor);
                Children.Add(upDoor);
                upDoor.Position = def.DoorUp.Value;
                upDoor.TargetRoom = RoomNode.Up.RideRoom;
            }

            // Left
            if (RoomNode.Left != null && def.DoorLeft != null && CreateThingClone<RideDoor>("RideDoorLeft") is RideDoor leftDoor)
            {
                doors.Add(leftDoor);
                Children.Add(leftDoor);
                leftDoor.Position = def.DoorLeft.Value;
                leftDoor.TargetRoom = RoomNode.Left.RideRoom;
            }

            // Right
            if (RoomNode.Right != null && def.DoorRight != null && CreateThingClone<RideDoor>("RideDoorRight") is RideDoor rightDoor)
            {
                doors.Add(rightDoor);
                Children.Add(rightDoor);
                rightDoor.Position = def.DoorRight.Value;
                rightDoor.TargetRoom = RoomNode.Right.RideRoom;
            }

            // Down
            if (RoomNode.Down != null)
            {
                if (def.DoorDown != null && CreateThingClone<RideDoor>("RideDoorDown") is RideDoor downDoor)
                {
                    doors.Add(downDoor);
                    Children.Add(downDoor);
                    downDoor.Position = def.DoorDown.Value;

                    if (RoomNode.Down != null)
                        downDoor.TargetRoom = RoomNode.Down.RideRoom;
                }
            }
        }

        // PrepareLights
        private void PrepareLights()
        {
            foreach (var door in doors)
            {
                var doorLight = AddLight(door.Name);
                //doorLight.Ambient = true;
                doorLight.Color = new Color(240, 181, 65) * .7f;
                doorLight.Position = door.BoundingBox.Center;
                doorLight.Scale = new(2, 7);
            }
        }

        // PrepareView
        private void PrepareView()
        {
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
                var viewName = $"View{Random.Shared.Next(1, index + 1)}";
                animation.AddFrame(viewName, 10000);
                foreground.Atlas = Atlas;
                foreground.DefaultImageName = viewName + "Foreground";
                foreground.ParallaxFactor = new(1.1f, 1);
                foreground.Position = new(0, 15);

                Children.Add(foreground);
            }
        }

        #endregion

        #region Protected members

        // OnActivate
        protected override void OnActivate()
        {
            base.OnActivate();

            if (!RoomNode.Visited)
                RoomNode.Visited = true;

            Session.StatusHUD.MiniMap.CurrentRoom = RoomNode; ;
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();

            PrepareView();
            PrepareLights();
            LockDoorsAccordingly();
            DistributeBronzeKeys();
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
                if ((previousRoomIndex == -1 && door.DoorDirection == DoorDirection.Down) ||
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
