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
        private static readonly ChanceTable terrainTable = new();
        private static readonly Dictionary<RoomSize, Vector2[]> terrainVertices = [];

        // Static constructor
        static RideRoom()
        {
            terrainTable.Add(RoomSize.Small.ToString(), 1, 100);
            //terrainTable.Add(RoomSize.Medium.ToString(), 1, 100);

            terrainVertices[RoomSize.Small] = ReadOnlyPolygon.GetVertices("295,17;295,122;5,122;5,17");
            //terrainVertices[RoomSize.Medium] = ReadOnlyPolygon.GetVertices("324,18;324,120;7,120;7,18");
        }

        // Constructor
        public RideRoom(GameSession session, string name, int roomIndex, bool isLastRoom)
            : base(session, name, RoomKind.RideRoom, roomIndex, isLastRoom)
        {
            AllowGlobalLight = true;
        }

        #region Private members

        // DeployCoin
        private void DeployCoin()
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
                creatures[index].HasCoin = true;
            }
            else if (WalkArea != null)
            {
                var metaItem = MetaItem.FindNotNull(MetaItem.CoinItemName);
                var position = WalkArea != null ? WalkArea.RandomWalkablePoint() : BoundingBox.GetRandomPoint();
                Session.ObjectPools.Pickups.Get()?.Drop(this, position, metaItem);
            }
        }

        #endregion

        #region Protected members

        // GetRoomData
        protected override string GetRoomData(out Vector2[] walkAreaVertices)
        {
            if (terrainTable.GetValue(Random) is ChanceTableItem item)
            {
                var size = Enum.Parse<RoomSize>(item.Name);
                walkAreaVertices = terrainVertices[size];
                return item.Name;
            }
            else
            {
                walkAreaVertices = [];
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
                RightConnector = CreateRuntimeClone("RightTower") as IsometricProp;
                if (RightConnector != null)
                {
                    RightConnector.Position = new Vector2(CustomWidth - 9, RightConnector.BoundingBox.Height - 4);
                    MainGrid.ReserveSpace(RightConnector, false);
                    Children.Add(RightConnector);

                    if (CreateRuntimeClone("RightTowerPatch") is IsometricProp rightTowerPatch)
                    {
                        rightTowerPatch.Position = RightConnector.Position;
                        Children.Add(rightTowerPatch);
                    }

                    if (Session.ScriptLibrary.GetRoutine(RoutineNames.GotoNextRunRoom) is Script script)
                    {
                        var lt = RightConnector.BoundingBox.GetPoint(RectanglePoint.LeftTop);
                        Vector2[] vertices = ReadOnlyPolygon.GetVertices("35,44;27,50;19,44;23,41");

                        for (var i = 0; i < vertices.Length; i++)
                        {
                            vertices[i] += lt;
                        }

                        AddTriggerArea("NextRoom", script, null, true, true, false, null, vertices);
                    }
                }
            }

            // Exit Tunnel
            else
            {      
                this.RightConnector = Session.GetEntity<IsometricProp>("ExitRail");

                if (this.RightConnector != null)
                {
                    this.RightConnector.Position = new Vector2(CustomWidth, 0);
                    MainGrid.ReserveSpace(RightConnector, false);
                    Children.Add(this.RightConnector);

                    if (Session.GetEntity<IsometricProp>("ExitTunnel") is IsometricProp exitTunnel)
                    {
                        exitTunnel.Position = RightConnector.Position;
                        Children.Add(exitTunnel);
                    }

                    if (Session.GetEntity<IsometricProp>("ExitTunnelPatch") is IsometricProp exitTunnelPatch)
                    {
                        exitTunnelPatch.Position = RightConnector.Position;
                        exitTunnelPatch.Y += 30;
                        Children.Add(exitTunnelPatch);
                    }

                    if (Session.GetEntity<ExitRideCar>(nameof(ExitRideCar)) is ExitRideCar exitRideCar)
                    {
                        exitRideCar.Position = RightConnector.BoundingBox.GetPoint(RectanglePoint.LeftBottom, 15, -6);
                        MainGrid.ReserveSpace(exitRideCar, false);
                        Children.Add(exitRideCar);
                    }
                }
            }

            if (RightConnector != null)
            {
                if (CreateRuntimeClone(nameof(SaintPeregrine)) is SaintPeregrine statue)
                {
                    statue.Position = this.RightConnector.BoundingBox.GetPoint(RectanglePoint.LeftBottom, -5, -15);
                    if (RoomPhase == RunPhase.End)
                        statue.Position += new Vector2(20, -41);

                    MainGrid.ReserveSpace(statue, false);
                    Children.Add(statue);
                }
            }

            DecorationGrid.MarkOccupiedMargin("Margin", 1, 1, 1, 2);
            MainGrid.MarkOccupiedMargin("Margin", 1, 1, 1, 2);
        }

        // OnPopulateCompleted
        protected override void OnPopulateCompleted()
        {
            DeployCoin();
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

        // RightConnector
        public IsometricProp? RightConnector { get; private set; }
    }
}
