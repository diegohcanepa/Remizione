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
        private enum TerrainSize { TerrainSmall, TerrainMedium }

        private static readonly ChanceTable terrainTable = new();
        private static readonly Dictionary<string, Vector2[]> terrainVertices = [];

        // Static constructor
        static RideRoom()
        {
            terrainTable.Add(TerrainSize.TerrainSmall.ToString(), 1, 2000);
            terrainTable.Add(TerrainSize.TerrainMedium.ToString(), 1, 50);

            terrainVertices[TerrainSize.TerrainSmall.ToString()] = ReadOnlyPolygon.GetVertices("234,17;234,122;5,122;5,17");
            terrainVertices[TerrainSize.TerrainMedium.ToString()] = ReadOnlyPolygon.GetVertices("324,18;324,120;7,120;7,18");
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
                creatures[index].HasMagneticCard = true;
            }
            else if (WalkArea != null)
            {
                var metaItem = MetaItem.FindNotNull(MetaItem.MagneticCardName);
                var position = WalkArea != null ? WalkArea.RandomWalkablePoint() : BoundingBox.GetRandomPoint();
                Session.ObjectPools.Pickups.Get()?.Drop(this, position, metaItem, 1);
            }
        }

        #endregion

        #region Protected members

        // GetTerrainData
        protected override string GetTerrainData(out Vector2[] walkAreaVertices)
        {
            if (terrainTable.GetValue(Random) is ChanceTableItem item)
            {
                walkAreaVertices = terrainVertices[item.Name];
                return item.Name;
            }
            else
            {
                walkAreaVertices = Array.Empty<Vector2>();
                return string.Empty;
            }
        }

        // OnLoad
        protected override void OnLoad()
        {
            base.OnLoad();
            AudioManager.Music.PlayTag("Ride");

            if (RoomPhase == RunPhase.Start)
                Session.AwaitRoutine(RoutineNames.IncomingRideCarIntro);
        }

        // OnPopulating
        protected override void OnPopulating()
        {
            if (RoomPhase == RunPhase.Start)
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
                    MainGrid.ReserveSpace(LeftTower, false);
                    Children.Add(this.LeftTower);

                    if (CreateRuntimeClone("LeftTowerPatch") is IsometricProp leftTowerPatch)
                    {
                        leftTowerPatch.Position = LeftTower.Position;
                        Children.Add(leftTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoPreviousRunRoom) is Script script)
                    {
                        Vector2[] vertices = ReadOnlyPolygon.GetVertices("18,39;15,49;5,45;6,39");
                        AddTriggerArea("PreviousRoom", script, null, true, true, false, null, vertices);
                    }
                }
            }

            // Right tower
            if (RoomPhase != RunPhase.End)
            {
                RightTower = CreateRuntimeClone("RightTower") as IsometricProp;
                if (RightTower != null)
                {
                    RightTower.Position = new Vector2(CustomWidth - 9, RightTower.BoundingBox.Height - 4);
                    MainGrid.ReserveSpace(RightTower, false);
                    Children.Add(RightTower);

                    if (CreateRuntimeClone("RightTowerPatch") is IsometricProp rightTowerPatch)
                    {
                        rightTowerPatch.Position = RightTower.Position;
                        Children.Add(rightTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoNextRunRoom) is Script script)
                    {
                        var lt = RightTower.BoundingBox.GetPoint(RectanglePoint.LeftTop);
                        Vector2[] vertices = ReadOnlyPolygon.GetVertices("35,44;27,50;19,44;23,41");

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
                    MainGrid.ReserveSpace(RightTower, false);
                    Children.Add(this.RightTower);

                    if (Session.GetEntity<ExitRideCar>(nameof(ExitRideCar)) is ExitRideCar exitRideCar)
                    {
                        exitRideCar.Position = RightTower.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, 9);
                        MainGrid.ReserveSpace(exitRideCar, false);
                        Children.Add(exitRideCar);
                    }
                }
            }

            if (RightTower != null)
            {
                if (CreateRuntimeClone(nameof(CardReader)) is CardReader cardReader)
                {
                    cardReader.Position = this.RightTower.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 0, -20);
                    if (RoomPhase == RunPhase.End)
                        cardReader.Position += new Vector2(20, -5);

                    MainGrid.ReserveSpace(cardReader, false);
                    Children.Add(cardReader);
                }
            }

            DecorationGrid.MarkOccupiedMargin("Margin", 0, 1, 0, 2);
            MainGrid.MarkOccupiedMargin("Margin", 0, 1, 0, 2);
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
