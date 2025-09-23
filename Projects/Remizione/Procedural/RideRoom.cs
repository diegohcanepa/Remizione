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
                    MainGrid.ReserveSpace(entranceRail);
                    Children.Add(entranceRail);
                }
            }
            else
            {
                // Left tower
                LeftTower = CreateRuntimeClone("LeftTower") as IsometricProp;
                if (LeftTower != null)
                {
                    MainGrid.ReserveSpace(LeftTower);
                    Children.Add(this.LeftTower);

                    if (CreateRuntimeClone("LeftTowerPatch") is IsometricProp leftTowerPatch)
                    {
                        leftTowerPatch.Position = LeftTower.Position;
                        Children.Add(leftTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoPreviousRunRoom) is Script script)
                    {
                        Vector2[] vertices = [new(11, 39), new(22, 39), new(22, 48), new(11, 48)];
                        AddTriggerArea("PreviousRoom", script, null, true, true, false, null, vertices);
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
                    MainGrid.ReserveSpace(RightTower);
                    Children.Add(RightTower);

                    if (CreateRuntimeClone("RightTowerPatch") is IsometricProp rightTowerPatch)
                    {
                        rightTowerPatch.Position = RightTower.Position;
                        Children.Add(rightTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoNextRunRoom) is Script script)
                    {
                        var lt = RightTower.BoundingBox.GetPoint(RectanglePoint.LeftTop);
                        Vector2[] vertices = [new(25, 39), new(36, 39), new(36, 48), new(25, 48)];

                        for (var i = 0; i < vertices.Length; i++)
                        {
                            vertices[i] += lt;
                        }

                        AddTriggerArea("NextRoom", script, null, true, true, false, null, vertices);
                    }
                }
            }

            // Exit tower
            else
            {
                this.RightTower = CreateRuntimeClone("ExitTower") as IsometricProp;
                if (this.RightTower != null)
                {
                    this.RightTower.Position = new Vector2(CustomWidth - 22, RightTower.BoundingBox.Height - 12);
                    Children.Add(this.RightTower);

                    if (Session.GetEntity<ExitRideCar>(nameof(ExitRideCar)) is ExitRideCar exitRideCar)
                    {
                        Children.Add(exitRideCar);
                        exitRideCar.Position = RightTower.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 9);
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
