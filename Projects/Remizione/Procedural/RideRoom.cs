using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        private const string MagneticCardName = "MagneticCard";

        // Constructor
        public RideRoom(GameSession session, string name, int roomIndex, bool isLastRoom)
            : base(session, name, RoomKind.RideRoom, roomIndex, isLastRoom)
        {
        }

        #region Private members

        // DeployMagneticCard
        private void DeployMagneticCard()
        {
            var creatures = new List<Creature>();
            foreach (var thing in Children)
            {
                if (thing is Creature creature)
                    creatures.Add(creature);
            }

            var index = creatures.RandomIndex();

            if (index >= 0)
            {
                creatures[index].HasMagneticCard = true;
            }
            else if (WalkArea != null)
            {
                var metaItem = MetaItem.FindNotNull(MagneticCardName);
                var position = WalkArea != null ? WalkArea.RandomWalkablePoint() : BoundingBox.GetRandomPoint();
                Session.ObjectPools.Pickups.Get()?.Drop(this, metaItem, position);
            }
        }

        #endregion

        #region Protected members

        // RequiresPersistence
        protected override bool RequiresPersistence => false;

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AudioManager.Music.PlayTag("Ride");

            if (RoomPosition == RoomPosition.First)
                Session.AwaitRoutine(RoutineNames.IncomingRideCarIntro);
        }

        // OnPopulate
        protected override void OnPopulate()
        {
            if (RoomPosition == RoomPosition.First)
            {
                // Entrance rail
                if (Session.GetEntity<GameThing>("EntranceRail") is GameThing entranceRail)
                {
                    var sizeInCells = entranceRail.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                        Children.Add(entranceRail);
                }
            }
            else
            {
                // Left tower
                if (Session.GetEntity<IsometricProp>("LeftTower") is IsometricProp leftTower)
                {
                    var sizeInCells = leftTower.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                    {
                        this.LeftTower = CreateRuntimeClone(leftTower.StaticName) as IsometricProp;
                        Children.Add(leftTower);
                    }
                }
            }

            // Right tower
            if (RoomPosition != RoomPosition.Last)
            {
                if (Session.GetEntity<IsometricProp>("RightTower") is IsometricProp rightTower)
                {
                    var sizeInCells = rightTower.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                    {
                        this.RightTower = CreateRuntimeClone(rightTower.StaticName) as IsometricProp;
                        if (this.RightTower != null)
                        {
                            this.RightTower.Position = new Vector2(CustomWidth - 19, rightTower.BoundingBox.Height + 6);
                            Children.Add(this.RightTower);
                        }
                    }
                }
            }

            // Exit tower
            else
            {
                if (Session.GetEntity<IsometricProp>("ExitTower") is IsometricProp exitTower)
                {
                    var sizeInCells = exitTower.GetRequiredGridSpace(ProceduralRoomGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                    {
                        this.RightTower = CreateRuntimeClone(exitTower.StaticName) as IsometricProp;
                        if (this.RightTower != null)
                        {
                            this.RightTower.Position = new Vector2(CustomWidth - 22, exitTower.BoundingBox.Height + 6);
                            Children.Add(this.RightTower);

                            if (Session.GetEntity<ExitRideCar>(nameof(ExitRideCar)) is ExitRideCar exitRideCar)
                            {
                                Children.Add(exitRideCar);
                                exitRideCar.Position = RightTower.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 9);
                            }
                        }
                    }
                }
            }

            if (RightTower != null)
            {
                if (CreateRuntimeClone(nameof(CardReader)) is CardReader cardReader)
                {
                    cardReader.Position = this.RightTower.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -20);

                    if (RoomPosition == RoomPosition.Last)
                        cardReader.Position += new Vector2(20, -5);

                    Children.Add(cardReader);
                }
            }
        }

        // OnPopulateCompleted
        protected override void OnPopulateCompleted()
        {
            DeployMagneticCard();
        }

        #endregion

        // LeftTower
        public IsometricProp? LeftTower { get; private set; }

        // OpenRightTower
        [ScriptMethod]
        public void OpenRightTower()
        {
            AnimationPlayer.Play("Open", false);
        }

        // RightTower
        public IsometricProp? RightTower { get; private set; }
    }
}
