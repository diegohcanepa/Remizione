using Adberration.Scripting;
using Engendro;
using Engendro.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Remizione
{
    /// <summary>
    /// RideRoom
    /// </summary>
    public sealed class RideRoom : ProceduralRoom
    {
        private static readonly ChanceTable terrainTable = new();

        // Static constructor
        static RideRoom()
        {
            terrainTable.Add("TerrainSmall", 200);
            terrainTable.Add("TerrainMedium", 50);
            terrainTable.Add("TerrainLarge", 25);
            terrainTable.Add("TerrainExtraLarge", 10);
        }

        // Constructor
        public RideRoom(GameSession session, string name, int roomIndex, bool isLastRoom)
            : base(session, name, RoomKind.RideRoom, roomIndex, isLastRoom)
        {
        }

        #region Private members

        // DeployPainCard
        private void DeployPainCard()
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
                creatures[index].HasPainCard = true;
            }
            else if (WalkArea != null)
            {
                var metaItem = MetaItem.FindNotNull(MetaItem.MagneticCardName);
                var position = WalkArea != null ? WalkArea.RandomWalkablePoint() : BoundingBox.GetRandomPoint();
                Session.ObjectPools.Pickups.Get()?.Drop(this, metaItem, position);
            }
        }

        #endregion

        #region Protected members

        // GetTerrainImageName
        protected override string GetTerrainImageName() => terrainTable.GetValue(Random);

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
                    var sizeInCells = entranceRail.GetRequiredGridSpace(MainGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out int col, out int row))
                        Children.Add(entranceRail);
                }
            }
            else
            {
                // Left tower
                if (Session.GetEntity<IsometricProp>("LeftTower") is IsometricProp leftTower)
                {
                    var sizeInCells = leftTower.GetRequiredGridSpace(MainGrid.CellSize);
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
                RightTower = CreateRuntimeClone("RightTower") as IsometricProp;
                if (RightTower != null)
                {
                    RightTower.Position = new Vector2(CustomWidth - 9, RightTower.BoundingBox.Height - 4);
                    MainGrid.ReserveSpace(RightTower.BoundingBox.ToRectangle());
                    Children.Add(this.RightTower);
                }
            }

            // Exit tower
            else
            {
                if (Session.GetEntity<IsometricProp>("ExitTower") is IsometricProp exitTower)
                {
                    var sizeInCells = exitTower.GetRequiredGridSpace(MainGrid.CellSize);
                    if (MainGrid.TryReserveSpace(sizeInCells, out _, out _))
                    {
                        this.RightTower = CreateRuntimeClone(exitTower.StaticName) as IsometricProp;
                        if (this.RightTower != null)
                        {
                            this.RightTower.Position = new Vector2(CustomWidth - 22, exitTower.BoundingBox.Height - 12);
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
            DeployPainCard();
        }

        // RequiresPersistence
        protected override bool RequiresPersistence => false;

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
